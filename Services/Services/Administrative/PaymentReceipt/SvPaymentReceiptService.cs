using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using System.Text;
using Infrastructure.Services.Administrative.EmailServices;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Domain.Entities.Administration;
// Reemplaza este using por el que apunte a tu nuevo AppDbContext:
using Infrastructure.Persistence.AppDbContext;

namespace Infrastructure.Services.Administrative.PaymentReceiptService
{
    public class SvPaymentReceipt : ISvPaymentReceipt
    {
        private readonly ILogger<SvPaymentReceipt> _logger;
        private readonly AppDbContext _context;    // <-- Cambiado a AppDbContext
        private readonly IMapper _mapper;
        private readonly ISvEmailService _emailService;

        public SvPaymentReceipt(
            AppDbContext context,             // <-- Ajustado aquí
            IMapper mapper,
            ISvEmailService emailService,
            ILogger<SvPaymentReceipt> logger)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        // Crear el comprobante de pago
        public async Task<PaymentReceiptDto> CreatePaymentReceiptAsync(PaymentReceiptCreateDto paymentReceiptCreateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var paymentReceipt = _mapper.Map<PaymentReceipt>(paymentReceiptCreateDto);
                paymentReceipt.CreatedAt = DateTime.UtcNow;

                decimal totalExtraHoursAmount = paymentReceiptCreateDto.ExtraHourRate * paymentReceiptCreateDto.Overtime;
                paymentReceipt.TotalExtraHoursAmount = totalExtraHoursAmount;

                // Calcular el monto bruto
                paymentReceipt.GrossAmount = paymentReceiptCreateDto.Salary + totalExtraHoursAmount;
                // Asignar GrossIncome
                paymentReceipt.GrossIncome = paymentReceipt.GrossAmount;
                // Calcular deducciones
                paymentReceipt.TotalDeductions = paymentReceiptCreateDto.DeductionsList?.Sum(d => d.Amount) ?? 0;
                // Monto neto
                paymentReceipt.NetAmount = paymentReceipt.GrossIncome - paymentReceipt.TotalDeductions;

                // Guardar en DB
                await _context.PaymentReceipts.AddAsync(paymentReceipt);
                await _context.SaveChangesAsync();

                if (paymentReceiptCreateDto.DeductionsList != null && paymentReceiptCreateDto.DeductionsList.Any())
                {
                    var deductions = _mapper.Map<List<Deduction>>(paymentReceiptCreateDto.DeductionsList);
                    foreach (var deduction in deductions)
                    {
                        deduction.PaymentReceiptId = paymentReceipt.Id;
                    }
                    await _context.Deductions.AddRangeAsync(deductions);
                }

                await _context.SaveChangesAsync();

                // Recargar PaymentReceipt con las relaciones
                var fullPaymentReceipt = await _context.PaymentReceipts
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.Profession)
                    .Include(r => r.Employee.TypeOfSalary)
                    .Include(r => r.DeductionsList)
                    .FirstOrDefaultAsync(r => r.Id == paymentReceipt.Id);

                if (fullPaymentReceipt == null)
                {
                    throw new Exception("Error al cargar el comprobante de pago después de la creación.");
                }

                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(fullPaymentReceipt);

                await transaction.CommitAsync();
                return paymentReceiptDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al crear el comprobante de pago: " + ex.Message, ex);
            }
        }

        // Obtener recibos de pago por empleado
        public async Task<IEnumerable<PaymentReceiptDto>> GetPaymentReceiptsByEmployeeAsync(int employeeDni)
        {
            var receipts = await _context.PaymentReceipts
                .Include(r => r.Employee)
                    .ThenInclude(e => e.Profession)
                .Include(r => r.Employee.TypeOfSalary)
                .Include(r => r.DeductionsList)
                .Where(r => r.Id_Employee == employeeDni)
                .ToListAsync();

            return receipts.Select(r => new PaymentReceiptDto
            {
                Id = r.Id,
                EmployeeDni = r.EmployeeDni,
                EmployeeFullName = $"{r.Employee.First_Name} {r.Employee.Last_Name1} {r.Employee.Last_Name2}",
                EmployeeEmail = r.Employee?.Email,
                Profession = r.Employee?.Profession?.Name_Profession,
                SalaryType = r.Employee?.TypeOfSalary?.Name_TypeOfSalary,
                PaymentDate = r.PaymentDate,
                Salary = r.Salary,
                Overtime = r.Overtime,
                GrossIncome = r.GrossIncome,
                NetAmount = r.NetAmount,
                TotalExtraHoursAmount = r.TotalExtraHoursAmount,
                TotalDeductions = r.TotalDeductions,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt,
                DeductionsList = r.DeductionsList.Select(d => new DeductionDto
                {
                    Id = d.Id,
                    PaymentReceiptId = d.PaymentReceiptId,
                    Type = d.Type,
                    Amount = d.Amount
                }).ToList()
            }).ToList();
        }

        // Obtener recibo de pago por ID
        public async Task<PaymentReceiptDto> GetPaymentReceiptByIdAsync(int id)
        {
            var receipt = await _context.PaymentReceipts
                .Include(r => r.Employee)
                    .ThenInclude(e => e.Profession)
                .Include(r => r.Employee.TypeOfSalary)
                .Include(r => r.DeductionsList)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
                throw new KeyNotFoundException($"Comprobante de pago con ID {id} no encontrado.");

            return _mapper.Map<PaymentReceiptDto>(receipt);
        }

        // Enviar el comprobante de pago por correo electrónico
        public async Task SendPaymentReceiptByEmailAsync(int receiptId)
        {
            var receipt = await GetPaymentReceiptByIdAsync(receiptId);
            var employeeEmail = receipt.EmployeeEmail;

            if (string.IsNullOrEmpty(employeeEmail))
            {
                throw new InvalidOperationException("El empleado no tiene un correo registrado.");
            }

            var pdfStream = await GeneratePaymentReceiptPdf(receipt);

            await _emailService.SendEmailWithAttachmentAsync(
                employeeEmail,
                "Comprobante de Pago",
                "Adjunto está su comprobante de pago.",
                pdfStream,
                "ComprobantePago.pdf"
            );
        }

        // Generar PDF del comprobante de pago
        public async Task<MemoryStream> GeneratePaymentReceiptPdf(PaymentReceiptDto receiptDto)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ComprobantePagoTemplate.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"No se encontró la plantilla HTML en la ruta: {templatePath}");
            }

            string htmlTemplate = await File.ReadAllTextAsync(templatePath);
            var deduccionesHtml = new StringBuilder();
            deduccionesHtml.Append("<table><thead><tr><th>Tipo de Deducción</th><th>Monto</th></tr></thead><tbody>");

            if (receiptDto.DeductionsList != null && receiptDto.DeductionsList.Any())
            {
                foreach (var deduction in receiptDto.DeductionsList)
                {
                    deduccionesHtml.Append($"<tr><td>{deduction.Type}</td><td>₡{deduction.Amount:N2}</td></tr>");
                }
            }
            else
            {
                deduccionesHtml.Append("<tr><td colspan='2'>No hay deducciones aplicadas</td></tr>");
            }

            deduccionesHtml.Append("</tbody></table>");

            // Reemplazar placeholders en el HTML
            string htmlContent = htmlTemplate
                .Replace("{{PaymentDate}}", receiptDto.PaymentDate.ToString("dd/MM/yyyy"))
                .Replace("{{EmployeeFullName}}", receiptDto.EmployeeFullName)
                .Replace("{{Profession}}", receiptDto.Profession)
                .Replace("{{SalaryType}}", receiptDto.SalaryType)
                .Replace("{{WorkedDays}}", receiptDto.WorkedDays.ToString())
                .Replace("{{Salary}}", receiptDto.Salary.ToString("N2"))
                .Replace("{{Overtime}}", receiptDto.Overtime.ToString("N2"))
                .Replace("{{ExtraHourRate}}", receiptDto.ExtraHourRate.ToString("N2"))
                .Replace("{{DoublesHours}}", receiptDto.DoublesHours.ToString("N2"))
                .Replace("{{DoubleExtras}}", receiptDto.DoubleExtras.ToString("N2"))
                .Replace("{{NightHours}}", receiptDto.NightHours.ToString("N2"))
                .Replace("{{MixedHours}}", receiptDto.MixedHours.ToString("N2"))
                .Replace("{{MandatoryHolidays}}", receiptDto.MandatoryHolidays.ToString("N2"))
                .Replace("{{MandatoryHolidaysAmount}}", (receiptDto.MandatoryHolidays * receiptDto.Salary / 30).ToString("N2"))
                .Replace("{{Adjustments}}", receiptDto.Adjustments.ToString("N2"))
                .Replace("{{Incapacity}}", receiptDto.Incapacity.ToString("N2"))
                .Replace("{{Absence}}", receiptDto.Absence.ToString("N2"))
                .Replace("{{VacationDays}}", receiptDto.VacationDays.ToString("N2"))
                .Replace("{{VacationAmount}}", (receiptDto.VacationDays * receiptDto.Salary / 30).ToString("N2"))
                .Replace("{{GrossIncome}}", receiptDto.GrossIncome.ToString("N2"))
                .Replace("{{NetAmount}}", receiptDto.NetAmount.ToString("N2"))
                .Replace("{{TotalExtraHoursAmount}}", receiptDto.TotalExtraHoursAmount.ToString())
                .Replace("{{DeduccionesTable}}", deduccionesHtml.ToString());

            var pdfStream = new MemoryStream();

            try
            {
                var launchOptions = new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH"),
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
                };

                var browser = await Puppeteer.LaunchAsync(launchOptions);
                var page = await browser.NewPageAsync();
                await page.SetContentAsync(htmlContent);

                var pdfOptions = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true
                };

                var pdfBytes = await page.PdfDataAsync(pdfOptions);
                await pdfStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                pdfStream.Position = 0;

                await browser.CloseAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al convertir HTML a PDF: " + ex.Message);
            }

            return pdfStream;
        }
    }
}
