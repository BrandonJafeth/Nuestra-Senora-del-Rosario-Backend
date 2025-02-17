using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Infrastructure.Persistence.AppDbContext;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Services.Informative.FormDonationService;
using Infrastructure.Services.Informative.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Tests.Helpers;
using Xunit;

namespace MyProject.Tests.Services
{
    public class FormDonationServiceTests
    {
        // Test de validación exitoso (caso válido).
        [Fact]
        public async Task CreateFormDonationAsync_WithValidDto_AddsDonationToContext()
        {
            // Arrange: crear un contexto en memoria con un nombre único.
            var dbName = Guid.NewGuid().ToString();
            using var context = AppDbContextInMemoryFactory.Create(dbName);

            // Configurar el mapper utilizando Moq.
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Domain.Entities.Informative.FormDonation>(It.IsAny<FormDonationCreateDto>()))
                .Returns((FormDonationCreateDto dto) => new Domain.Entities.Informative.FormDonation
                {
                    Dn_Name = dto.Dn_Name,
                    Dn_Lastname1 = dto.Dn_Lastname1,
                    Dn_Lastname2 = dto.Dn_Lastname2,
                    Dn_Cedula = dto.Dn_Cedula,
                    Dn_Email = dto.Dn_Email,
                    Dn_Phone = dto.Dn_Phone,
                    Delivery_date = dto.Delivery_date,
                    Id_DonationType = dto.Id_DonationType,
                    Id_MethodDonation = dto.Id_MethodDonation
                    // Se establecerá Id_Status en el servicio.
                });

            // Usamos la instancia real del validator.
            var validator = new FormDonationCreateDtoValidator();

            // Configurar el logger (mock).
            var loggerMock = new Mock<ILogger<SvFormDonationService>>();

            // Crear la instancia del servicio (SUT).
            var service = new SvFormDonationService(context, mapperMock.Object, validator, loggerMock.Object);

            // Configurar un DTO de prueba válido.
            var dtoCreate = new FormDonationCreateDto
            {
                Dn_Name = "Juan",
                Dn_Lastname1 = "Pérez",
                Dn_Lastname2 = "Gómez",
                Dn_Cedula = 123456789, // 9 dígitos válidos.
                Dn_Email = "juan@example.com",
                Dn_Phone = "12345678", // 8 dígitos.
                Delivery_date = DateTime.Today.AddDays(1), // Fecha futura, válida.
                Id_DonationType = 1,
                Id_MethodDonation = 2
            };

            // Act: ejecutar el método a probar.
            await service.CreateFormDonationAsync(dtoCreate);

            // Assert: verificar que la donación se agregó al contexto.
            var donation = await context.FormDonations.FirstOrDefaultAsync();
            donation.Should().NotBeNull();
            donation.Dn_Name.Should().Be("Juan");
            donation.Id_Status.Should().Be(1); // Se espera que se establezca el estado "Pendiente".
        }

        // Test parametrizado para casos inválidos (validaciones).
        [Theory]
        [MemberData(nameof(GetInvalidDonationDtos))]
        public async Task CreateFormDonationAsync_WithInvalidDto_ThrowsArgumentException(FormDonationCreateDto invalidDto, string expectedErrorMessage)
        {
            // Arrange: crear un contexto en memoria con un nombre único.
            var dbName = Guid.NewGuid().ToString();
            using var context = AppDbContextInMemoryFactory.Create(dbName);

            // Para estos tests, el mapper no se utilizará (la validación falla primero).
            var mapperMock = new Mock<IMapper>();

            // Usamos la instancia real del validator.
            var validator = new FormDonationCreateDtoValidator();

            // Configurar el logger (mock).
            var loggerMock = new Mock<ILogger<SvFormDonationService>>();

            var service = new SvFormDonationService(context, mapperMock.Object, validator, loggerMock.Object);

            // Act & Assert: se espera que se lance una excepción con el mensaje correspondiente.
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateFormDonationAsync(invalidDto));
            exception.Message.Should().Contain(expectedErrorMessage);
        }

        // Datos de prueba con los distintos escenarios inválidos.
        public static IEnumerable<object[]> GetInvalidDonationDtos()
        {
            // Caso: Nombre vacío.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789, // 9 dígitos válidos para este caso.
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "El nombre es requerido."
            };

            // Caso: Apellido vacío.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "El apellido es requerido."
            };

            // Caso: Cédula menor o igual a cero.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 0,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "La cédula debe ser un número de 9 dígitos."
            };

            // Caso: Cédula con 8 dígitos (inválida).
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 12345678, // 8 dígitos
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "La cédula debe ser un número de 9 dígitos."
            };

            // Caso: Correo vacío.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "Un correo electrónico válido es requerido."
            };

            // Caso: Correo no válido.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "notavalidemail",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "Un correo electrónico válido es requerido."
            };

            // Caso: Teléfono vacío.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "El teléfono es requerido."
            };

            // Caso: Teléfono con número incorrecto de dígitos (no 8 dígitos).
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "1234567", // 7 dígitos (inválido)
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "El teléfono debe contener 8 dígitos."
            };

            // Caso: Fecha de entrega en el pasado.
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today.AddDays(-1), // Fecha en el pasado
                    Id_DonationType = 1,
                    Id_MethodDonation = 1
                },
                "La fecha de entrega no puede ser en el pasado."
            };

            // Caso: Id_DonationType inválido (<= 0).
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 0,
                    Id_MethodDonation = 1
                },
                "El tipo de donación es requerido."
            };

            // Caso: Id_MethodDonation inválido (<= 0).
            yield return new object[]
            {
                new FormDonationCreateDto
                {
                    Dn_Name = "Juan",
                    Dn_Lastname1 = "Perez",
                    Dn_Lastname2 = "Gomez",
                    Dn_Cedula = 123456789,
                    Dn_Email = "juan@example.com",
                    Dn_Phone = "12345678",
                    Delivery_date = DateTime.Today,
                    Id_DonationType = 1,
                    Id_MethodDonation = 0
                },
                "El método de donación es requerido."
            };
        }
    }
}
