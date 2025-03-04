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
    public class ProductCreateDtoValidatorTests
    {
        private readonly ProductCreateDTOValidator _validator;

        public ProductCreateDtoValidatorTests()
        {
            _validator = new ProductCreateDTOValidator();
        }

        // ✅ Caso exitoso
        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new ProductCreateDTO
            {
                Name = "Producto A",
                CategoryID = 1,
                UnitOfMeasureID = 2,
                InitialQuantity = 10
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // 🚨 Validación del Nombre: requerido y longitud máxima
        [Theory]
        [InlineData("", "El nombre es requerido.")]
        [InlineData("NombreQueExcedeElLimiteDeCienCaracteres..................................................................................", "El nombre no debe exceder 100 caracteres.")]
        public void Should_Have_Error_When_Name_Is_Invalid(string name, string expectedMessage)
        {
            var dto = new ProductCreateDTO
            {
                Name = name,
                CategoryID = 1,
                UnitOfMeasureID = 2,
                InitialQuantity = 10
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación de CategoryID: debe ser mayor a 0
        [Theory]
        [InlineData(0, "La categoría es obligatoria y debe ser válida.")]
        [InlineData(-1, "La categoría es obligatoria y debe ser válida.")]
        public void Should_Have_Error_When_CategoryID_Is_Invalid(int categoryId, string expectedMessage)
        {
            var dto = new ProductCreateDTO
            {
                Name = "Producto A",
                CategoryID = categoryId,
                UnitOfMeasureID = 2,
                InitialQuantity = 10
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.CategoryID)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación de UnitOfMeasureID: debe ser mayor a 0
        [Theory]
        [InlineData(0, "La unidad de medida es obligatoria y debe ser válida.")]
        [InlineData(-1, "La unidad de medida es obligatoria y debe ser válida.")]
        public void Should_Have_Error_When_UnitOfMeasureID_Is_Invalid(int unitId, string expectedMessage)
        {
            var dto = new ProductCreateDTO
            {
                Name = "Producto A",
                CategoryID = 1,
                UnitOfMeasureID = unitId,
                InitialQuantity = 10
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.UnitOfMeasureID)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación de TotalQuantity: no puede ser negativo
        [Theory]
        [InlineData(-5, "La cantidad inicial debe ser mayor o igual a cero.")]
        public void Should_Have_Error_When_TotalQuantity_Is_Invalid(int totalQuantity, string expectedMessage)
        {
            var dto = new ProductCreateDTO
            {
                Name = "Producto A",
                CategoryID = 1,
                UnitOfMeasureID = 2,
                InitialQuantity = totalQuantity
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.InitialQuantity)
                  .WithErrorMessage(expectedMessage);
        }
    }

    public class ProductPatchDtoValidatorTests
    {
        private readonly ProductPatchDtoValidator _validator;

        public ProductPatchDtoValidatorTests()
        {
            _validator = new ProductPatchDtoValidator();
        }

        // ✅ Caso exitoso cuando se proporcionan valores válidos
        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new ProductPatchDto
            {
                Name = "Producto B",
                CategoryID = 1,
                UnitOfMeasureID = 2,
                TotalQuantity = 20
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        // Validación del Nombre en patch: si se envía (no es null) y es vacío o demasiado largo
        [Theory]
        [InlineData("", "El nombre no debe estar vacío.")]
        [InlineData("NombreQueExcedeElLimiteDeCienCaracteres..................................................................................", "El nombre no debe exceder 100 caracteres.")]
        public void Should_Have_Error_When_Name_Is_Invalid(string name, string expectedMessage)
        {
            var dto = new ProductPatchDto
            {
                Name = name
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación de CategoryID en patch: si se envía, debe ser mayor a 0
        [Theory]
        [InlineData(0, "La categoría debe ser válida.")]
        [InlineData(-2, "La categoría debe ser válida.")]
        public void Should_Have_Error_When_CategoryID_Is_Invalid(int categoryId, string expectedMessage)
        {
            var dto = new ProductPatchDto
            {
                CategoryID = categoryId
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.CategoryID.Value)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación de UnitOfMeasureID en patch: si se envía, debe ser mayor a 0
        [Theory]
        [InlineData(0, "La unidad de medida debe ser válida.")]
        [InlineData(-3, "La unidad de medida debe ser válida.")]
        public void Should_Have_Error_When_UnitOfMeasureID_Is_Invalid(int unitId, string expectedMessage)
        {
            var dto = new ProductPatchDto
            {
                UnitOfMeasureID = unitId
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.UnitOfMeasureID.Value)
                  .WithErrorMessage(expectedMessage);
        }

        // 🚨 Validación de TotalQuantity en patch: si se envía, no puede ser negativo
        [Theory]
        [InlineData(-10, "La cantidad total debe ser mayor o igual a cero.")]
        public void Should_Have_Error_When_TotalQuantity_Is_Invalid(int totalQuantity, string expectedMessage)
        {
            var dto = new ProductPatchDto
            {
                TotalQuantity = totalQuantity
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.TotalQuantity.Value)
                  .WithErrorMessage(expectedMessage);
        }
    }
}
