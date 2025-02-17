using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Validations.Informative;
using Xunit;

namespace Test.ServicesTest.Informative
{
    public class ApplicationFormCreateDtoValidatorTests
    {
        private readonly ApplicationFormCreateDtoValidator _validator;

        public ApplicationFormCreateDtoValidatorTests()
        {
            _validator = new ApplicationFormCreateDtoValidator();
        }

        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new ApplicationFormCreateDto
            {
                Name_AP = "Juan",
                LastName1_AP = "Perez",
                Cedula_AP = "123456789",
                Age_AP = 65,
                Location_AP = "San Jose",
                GuardianName = "Carlos",
                GuardianLastName1 = "Lopez",
                GuardianCedula = "987654321",
                GuardianEmail = "carlos@example.com",
                GuardianPhone = "88889999"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("", "El nombre del aplicante es requerido.")]
        [InlineData("EsteNombreEsMuyLargoYExcedeElLimiteDeCaracteresPermitidoPorqueTieneMasDeCienCaracteres.................", "El nombre no puede exceder los 100 caracteres.")]
        public void Should_Have_Error_When_Name_AP_Is_Invalid(string name, string expectedMessage)
        {
            // Arrange
            var dto = new ApplicationFormCreateDto
            {
                Name_AP = name,
                LastName1_AP = "Perez",
                Cedula_AP = "123456789",
                Age_AP = 65,
                Location_AP = "San Jose",
                GuardianName = "Carlos",
                GuardianLastName1 = "Lopez",
                GuardianCedula = "987654321",
                GuardianEmail = "carlos@example.com",
                GuardianPhone = "88889999"
            };

         
            var result = _validator.TestValidate(dto);

        
            result.ShouldHaveValidationErrorFor(x => x.Name_AP)
                  .WithErrorMessage(expectedMessage);
        }



        [Theory]
        [InlineData("", "La cédula del aplicante es requerida.")]
        [InlineData("12345", "La cédula debe tener entre 9 y 12 dígitos.")]
        public void Should_Have_Error_When_Cedula_AP_Is_Invalid(string cedula, string expectedMessage)
        {
            var dto = new ApplicationFormCreateDto
            {
                Name_AP = "Juan",
                LastName1_AP = "Perez",
                Cedula_AP = cedula,
                Age_AP = 65,
                Location_AP = "San Jose",
                GuardianName = "Carlos",
                GuardianLastName1 = "Lopez",
                GuardianCedula = "987654321",
                GuardianEmail = "carlos@example.com",
                GuardianPhone = "88889999"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Cedula_AP)
                  .WithErrorMessage(expectedMessage);
        }

        [Fact]
        public void Should_Have_Error_When_Age_AP_Is_Invalid()
        {
            var dto = new ApplicationFormCreateDto
            {
                Name_AP = "Juan",
                LastName1_AP = "Perez",
                Cedula_AP = "123456789",
                Age_AP = 60, // Menor a 65
                Location_AP = "San Jose",
                GuardianName = "Carlos",
                GuardianLastName1 = "Lopez",
                GuardianCedula = "987654321",
                GuardianEmail = "carlos@example.com",
                GuardianPhone = "88889999"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Age_AP)
                  .WithErrorMessage("La edad del aplicante debe ser mayor o igual a 65 años.");
        }

        [Fact]
        public void Should_Have_Error_When_GuardianPhone_Is_Invalid()
        {
            var dto = new ApplicationFormCreateDto
            {
                Name_AP = "Juan",
                LastName1_AP = "Perez",
                Cedula_AP = "123456789",
                Age_AP = 65,
                Location_AP = "San Jose",
                GuardianName = "Carlos",
                GuardianLastName1 = "Lopez",
                GuardianCedula = "987654321",
                GuardianEmail = "carlos@example.com",
                GuardianPhone = "12345" // No tiene 8 dígitos
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.GuardianPhone)
                  .WithErrorMessage("El teléfono debe contener 8 dígitos.");
        }
    }
}
