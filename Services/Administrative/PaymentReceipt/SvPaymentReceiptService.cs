using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Administration;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.Administrative.EmailServices;
using Services.MyDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Services.Administrative.PaymentReceiptService
{
    public class SvPaymentReceipt : ISvPaymentReceipt
    {
        private readonly ILogger<SvPaymentReceipt> _logger;
        private readonly AdministrativeContext _context;
        private readonly IMapper _mapper;
        private readonly ISvEmailService _emailService;

        public SvPaymentReceipt(AdministrativeContext context, IMapper mapper, ISvEmailService emailService, ILogger<SvPaymentReceipt> logger)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

    public async Task<PaymentReceiptDto> CreatePaymentReceiptAsync(PaymentReceiptCreateDto paymentReceiptCreateDto)
{
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // Crear el comprobante de pago
        var paymentReceipt = _mapper.Map<PaymentReceipt>(paymentReceiptCreateDto);
        paymentReceipt.CreatedAt = DateTime.UtcNow;

        // Calcular el total de deducciones
        paymentReceipt.TotalDeductions = paymentReceiptCreateDto.DeductionsList.Sum(d => d.Amount);

        // Calcular el monto neto
        paymentReceipt.NetAmount = paymentReceipt.GrossAmount - paymentReceipt.TotalDeductions;

        await _context.PaymentReceipts.AddAsync(paymentReceipt);
        await _context.SaveChangesAsync();

        // Registrar las deducciones
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
        await transaction.CommitAsync();

        return _mapper.Map<PaymentReceiptDto>(paymentReceipt);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        throw new Exception("Error al crear el comprobante de pago: " + ex.Message);
    }
}



        public async Task<IEnumerable<PaymentReceiptDto>> GetPaymentReceiptsByEmployeeAsync(int employeeDni)
        {
            var receipts = await _context.PaymentReceipts
                .Include(r => r.Employee)                  // Incluye el empleado
                .ThenInclude(e => e.Profession)            // Incluye la profesión del empleado
                .Include(r => r.Employee.TypeOfSalary)     // Incluye el tipo de salario del empleado
                .Include(r => r.DeductionsList)            // Incluye la lista de deducciones
                .Where(r => r.EmployeeDni == employeeDni)  // Filtra por el DNI del empleado
                .ToListAsync();

            // Verificar los datos en la consola
            foreach (var receipt in receipts)
            {
                Console.WriteLine($"Empleado: {receipt.Employee?.First_Name} {receipt.Employee?.Last_Name1}, Profesión: {receipt.Employee?.Profession?.Name_Profession}, Tipo de salario: {receipt.Employee?.TypeOfSalary?.Name_TypeOfSalary}");
            }

            var result = receipts.Select(r => new PaymentReceiptDto
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
                GrossAmount = r.GrossAmount,
                NetAmount = r.NetAmount,
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

            return result;
        }



        public async Task<PaymentReceiptDto> GetPaymentReceiptByIdAsync(int id)
        {
            var receipt = await _context.PaymentReceipts
                .Include(r => r.Employee)              // Incluir el empleado
                .ThenInclude(e => e.Profession)        // Incluir la profesión del empleado
                .Include(r => r.Employee)              // Repetimos el Include para otra relación
                .ThenInclude(e => e.TypeOfSalary)      // Incluir el tipo de salario del empleado
                .Include(r => r.DeductionsList)        // Incluir la lista de deducciones
                .FirstOrDefaultAsync(r => r.Id == id); // Buscar por ID

            if (receipt == null)
                throw new KeyNotFoundException($"Comprobante de pago con ID {id} no encontrado.");

            // Usamos el logger para verificar que estamos obteniendo los datos correctos
            _logger.LogInformation($"Empleado: {receipt.Employee.First_Name} {receipt.Employee.Last_Name1}, " +
                $"Email: {receipt.Employee.Email}, Profesión: {receipt.Employee.Profession?.Name_Profession}, " +
                $"Tipo de Salario: {receipt.Employee.TypeOfSalary?.Name_TypeOfSalary}");

            return _mapper.Map<PaymentReceiptDto>(receipt);
        }





        public async Task SendPaymentReceiptByEmailAsync(int receiptId)
        {
            var receipt = await GetPaymentReceiptByIdAsync(receiptId);
            var employeeEmail = receipt.EmployeeEmail;

            if (string.IsNullOrEmpty(employeeEmail))
            {
                throw new InvalidOperationException("El empleado no tiene un correo registrado.");
            }

            var pdfStream = await GeneratePaymentReceiptPdf(receipt);

            await _emailService.SendEmailWithAttachmentAsync(employeeEmail, "Comprobante de Pago", "Adjunto está su comprobante de pago.", pdfStream, "ComprobantePago.pdf");
        }

        // Implementación de la generación del PDF
        public async Task<MemoryStream> GeneratePaymentReceiptPdf(PaymentReceiptDto receiptDto)
        {
            // Aquí debes generar el PDF en base al contenido de `receiptDto`.
            // Usarías una librería de PDF como iTextSharp, PdfSharp o cualquier otra.

            // Por ahora, un placeholder:
            var pdfStream = new MemoryStream();
            // Aquí iría el código de generación del PDF
            // Escribir los datos en el stream

            return pdfStream;
        }
    }
}
