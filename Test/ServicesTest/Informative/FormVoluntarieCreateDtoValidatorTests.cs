using System;
using FluentAssertions;
using FluentValidation;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Services.Informative.Validators;
using Infrastructure.Validations.Informative;
using Xunit;

namespace Test.ServicesTest.Informative
{
    public class FormVoluntarieCreateDtoValidatorTests
    {
        private readonly FormVoluntarieCreateDtoValidator _validator;

        public FormVoluntarieCreateDtoValidatorTests()
        {
            _validator = new FormVoluntarieCreateDtoValidator();
        }

        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            // Arrange: crear un DTO válido
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,   // 9 dígitos
                Vn_Email = "juan@example.com",
                Vn_Phone = "12345678",   // 8 dígitos
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            // Act: ejecutar la validación
            var result = _validator.Validate(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "El nombre es requerido.")]
        public void Should_Have_Error_When_Vn_Name_Is_Empty(string name, string expectedMessage)
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = name,
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "juan@example.com",
                Vn_Phone = "12345678",
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
        }

        [Theory]
        [InlineData(12345678, "La cédula debe ser un número de 9 dígitos.")]  // 8 dígitos
        public void Should_Have_Error_When_Vn_Cedula_Is_Invalid(int cedula, string expectedMessage)
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = cedula,
                Vn_Email = "juan@example.com",
                Vn_Phone = "12345678",
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
        }

        [Fact]
        public void Should_Have_Error_When_Vn_Email_Is_Invalid()
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "notavalidemail",
                Vn_Phone = "12345678",
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("correo electrónico válido"));
        }

        [Theory]
        [InlineData("", "El teléfono es requerido.")]
        public void Should_Have_Error_When_Vn_Phone_Is_Empty(string phone, string expectedMessage)
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "juan@example.com",
                Vn_Phone = phone,
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
        }

        [Theory]
        [InlineData("1234567", "El teléfono debe contener 8 dígitos.")]
        public void Should_Have_Error_When_Vn_Phone_Has_Invalid_Length(string phone, string expectedMessage)
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "juan@example.com",
                Vn_Phone = phone,   // 7 dígitos, inválido
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
        }

        [Fact]
        public void Should_Have_Error_When_Delivery_Date_Is_In_Past()
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "juan@example.com",
                Vn_Phone = "12345678",
                Delivery_Date = DateTime.Today.AddDays(-1), // Fecha pasada
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("La fecha de inicio no puede ser en el pasado"));
        }

        [Fact]
        public void Should_Have_Error_When_End_Date_Is_Not_After_Delivery_Date()
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "juan@example.com",
                Vn_Phone = "12345678",
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today, // Misma fecha, no es posterior
                VoluntarieTypeId = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("La fecha final debe ser posterior a la fecha de inicio."));
        }

        [Theory]
        [InlineData(0, "El tipo de voluntariado es requerido.")]
        public void Should_Have_Error_When_VoluntarieTypeId_Is_Invalid(int typeId, string expectedMessage)
        {
            var dto = new FormVoluntarieCreateDto
            {
                Vn_Name = "Juan",
                Vn_Lastname1 = "Perez",
                Vn_Lastname2 = "Gomez",
                Vn_Cedula = 123456789,
                Vn_Email = "juan@example.com",
                Vn_Phone = "12345678",
                Delivery_Date = DateTime.Today,
                End_Date = DateTime.Today.AddDays(1),
                VoluntarieTypeId = typeId
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage.Contains(expectedMessage));
        }
    }
}
