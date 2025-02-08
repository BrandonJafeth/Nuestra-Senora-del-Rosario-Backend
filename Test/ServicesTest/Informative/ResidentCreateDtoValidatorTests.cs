using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Validations.Administrative;
using Xunit;

namespace Test.ServicesTest.Administrative
{
    public class ResidentCreateDtoValidatorTests
    {
        private readonly ResidentCreateDtoValidator _validator;

        public ResidentCreateDtoValidatorTests()
        {
            _validator = new ResidentCreateDtoValidator();
        }

        // ✅ Prueba caso exitoso
        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new ResidentCreateDto
            {
                Name_RD = "Carlos",
                Lastname1_RD = "Perez",
                Lastname2_RD = "Gomez",
                Cedula_RD = "123456789",
                Sexo = "Masculino",
                FechaNacimiento = new DateTime(1950, 5, 20),
                Id_Guardian = 1,
                Id_Room = 2,
                EntryDate = DateTime.UtcNow,
                Id_DependencyLevel = 3,
                Location_RD = "San José, Costa Rica"
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // 🚨 Nombre vacío o demasiado largo
        [Theory]
        [InlineData("", "El nombre del residente es requerido.")]
        [InlineData("NombreDemasiadoLargoQueExcedeElLimiteDeCaracteresPermitidoPorqueTieneMasDeCienCaracteres.................", "El nombre no puede exceder los 100 caracteres.")]
        public void Should_Have_Error_When_Name_RD_Is_Invalid(string name, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Name_RD = name };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name_RD)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Cedula inválida
        [Theory]
        [InlineData("", "La cédula del residente es requerida.")]
        [InlineData("12345", "La cédula debe tener entre 9 y 12 dígitos.")]
        public void Should_Have_Error_When_Cedula_RD_Is_Invalid(string cedula, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Cedula_RD = cedula };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Cedula_RD)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación del Sexo (Solo "Masculino" o "Femenino")
        [Theory]
        [InlineData("", "El sexo del residente es requerido.")]
        [InlineData("Otro", "El sexo debe ser 'Femenino' o 'Masculino'.")]
        public void Should_Have_Error_When_Sexo_Is_Invalid(string sexo, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Sexo = sexo };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Sexo)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Fecha de nacimiento inválida (futura)
        [Fact]
        public void Should_Have_Error_When_FechaNacimiento_Is_In_Future()
        {
            var dto = new ResidentCreateDto { FechaNacimiento = DateTime.UtcNow.AddYears(1) };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.FechaNacimiento)
                  .WithErrorMessage("La fecha de nacimiento debe ser anterior a hoy.");
        }

        // 🚨 Validación del ID del Guardián (Mayor a 0)
        [Theory]
        [InlineData(0, "El ID del guardián es requerido y debe ser válido.")]
        [InlineData(-1, "El ID del guardián es requerido y debe ser válido.")]
        public void Should_Have_Error_When_Id_Guardian_Is_Invalid(int idGuardian, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Id_Guardian = idGuardian };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id_Guardian)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación del ID de la Habitación (Mayor a 0)
        [Theory]
        [InlineData(0, "El ID de la habitación es requerido y debe ser válido.")]
        [InlineData(-1, "El ID de la habitación es requerido y debe ser válido.")]
        public void Should_Have_Error_When_Id_Room_Is_Invalid(int idRoom, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Id_Room = idRoom };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id_Room)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación del ID del Nivel de Dependencia (Mayor a 0)
        [Theory]
        [InlineData(0, "El nivel de dependencia es requerido y debe ser válido.")]
        [InlineData(-1, "El nivel de dependencia es requerido y debe ser válido.")]
        public void Should_Have_Error_When_Id_DependencyLevel_Is_Invalid(int idDependencyLevel, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Id_DependencyLevel = idDependencyLevel };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Id_DependencyLevel)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Fecha de ingreso futura
        [Fact]
        public void Should_Have_Error_When_EntryDate_Is_In_Future()
        {
            var dto = new ResidentCreateDto { EntryDate = DateTime.UtcNow.AddDays(1) };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EntryDate)
                  .WithErrorMessage("La fecha de ingreso no puede ser en el futuro.");
        }

        // 🚨 Validación de localización vacía o demasiado larga
        [Theory]
        [InlineData("", "La localización del residente es requerida.")]
        [InlineData("UbicaciónMuyLargaQueExcedeElLimiteDeCaracteresPermitidoPorElSistema..........................................................................................................", "La localización no puede exceder los 250 caracteres.")]
        public void Should_Have_Error_When_Location_RD_Is_Invalid(string location, string expectedMessage)
        {
            var dto = new ResidentCreateDto { Location_RD = location };
            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Location_RD)
                  .WithErrorMessage(expectedMessage);
        }
    }
}
