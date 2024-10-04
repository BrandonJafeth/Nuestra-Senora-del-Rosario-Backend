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
                .Include(r => r.Employee)
                .Where(r => r.EmployeeDni == employeeDni)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PaymentReceiptDto>>(receipts);
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
