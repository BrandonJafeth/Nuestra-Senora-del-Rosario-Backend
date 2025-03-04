using FluentValidation.TestHelper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Validations.Admistrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ServicesTest.Administrative
{
    public class InventoryCreateDTOValidatorTests
    {
        private readonly InventoryCreateDTOValidator _validator;

        public InventoryCreateDTOValidatorTests()
        {
            _validator = new InventoryCreateDTOValidator();
        }

        // Caso exitoso: DTO válido
        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new InventoryCreateDTO
            {
                ProductID = 1,
                Quantity = 10,
                Date = DateTime.Now.AddDays(-1),
                MovementType = "Ingreso"
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // Error en ProductID: debe ser mayor a 0
        [Theory]
        [InlineData(0, "El ID del producto debe ser mayor a cero.")]
        [InlineData(-1, "El ID del producto debe ser mayor a cero.")]
        public void Should_Have_Error_When_ProductID_Is_Invalid(int productId, string expectedMessage)
        {
            var dto = new InventoryCreateDTO
            {
                ProductID = productId,
                Quantity = 10,
                Date = DateTime.Now.AddDays(-1),
                MovementType = "Ingreso"
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.ProductID)
                  .WithErrorMessage(expectedMessage);
        }

        // Error en Quantity: debe ser mayor a 0
        [Theory]
        [InlineData(0, "La cantidad debe ser mayor a cero.")]
        [InlineData(-5, "La cantidad debe ser mayor a cero.")]
        public void Should_Have_Error_When_Quantity_Is_Invalid(int quantity, string expectedMessage)
        {
            var dto = new InventoryCreateDTO
            {
                ProductID = 1,
                Quantity = quantity,
                Date = DateTime.Now.AddDays(-1),
                MovementType = "Ingreso"
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                  .WithErrorMessage(expectedMessage);
        }

        // Error en Date: no puede ser en el futuro
        [Fact]
        public void Should_Have_Error_When_Date_Is_In_Future()
        {
            var dto = new InventoryCreateDTO
            {
                ProductID = 1,
                Quantity = 10,
                Date = DateTime.Now.AddDays(1),
                MovementType = "Ingreso"
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Date)
                  .WithErrorMessage("La fecha no puede ser en el futuro.");
        }

        // Error en MovementType: requerido y debe ser "Ingreso" o "Egreso"
        [Theory]
        [InlineData("", "El tipo de movimiento es requerido.")]
        [InlineData("Otro", "El tipo de movimiento debe ser 'Ingreso' o 'Egreso'.")]
        public void Should_Have_Error_When_MovementType_Is_Invalid(string movementType, string expectedMessage)
        {
            var dto = new InventoryCreateDTO
            {
                ProductID = 1,
                Quantity = 10,
                Date = DateTime.Now.AddDays(-1),
                MovementType = movementType
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.MovementType)
                  .WithErrorMessage(expectedMessage);
        }
    }
}