using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;
using SIGEBI.Web.Controllers;
using SIGEBI.Web.Models;
using Xunit;

namespace SIGEBI.Test.Application;

public class LibroControllerTests
{
    [Fact]
    public async Task Index_maps_service_results_to_view_models()
    {
        // Arrange
        var libro = Libro.Create("Titulo", "Autor", 2, "123", "A1", DateTime.UtcNow);
        var serviceMock = new Mock<ILibroService>();
        serviceMock.Setup(s => s.BuscarAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Libro> { libro });

        var controller = new LibroController(serviceMock.Object);

        // Act
        var result = await controller.Index(null, null, CancellationToken.None) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeAssignableTo<List<LibroViewModel>>();
        var model = (List<LibroViewModel>)result.Model!;
        model.Should().ContainSingle();
        model[0].Titulo.Should().Be(libro.Titulo);
        model[0].EjemplaresDisponibles.Should().Be(libro.EjemplaresDisponibles);
    }

    [Fact]
    public async Task Create_returns_view_when_model_is_invalid()
    {
        var serviceMock = new Mock<ILibroService>();
        var controller = new LibroController(serviceMock.Object);
        controller.ModelState.AddModelError("Titulo", "Requerido");

        var model = new LibroViewModel();

        var result = await controller.Create(model, CancellationToken.None) as ViewResult;

        result.Should().NotBeNull();
        result!.Model.Should().Be(model);
        serviceMock.Verify(s => s.CrearAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
