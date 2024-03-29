﻿using BusinessSolutionShared;
using InquiryApp.Controllers;
using InquiryApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Diagnostics;

namespace InquiryApp.Tests;

public class HomeControllerTests
{
    private readonly HomeController _controller;
    private readonly Mock<IInquiryAppRepository> _mockRepo = new();
    private readonly Mock<IUrlHelper> _mockUrlHelper = new();
    private readonly Mock<IUrlHelperFactory> _mockUrlHelperFactory = new();
    private readonly TempDataDictionary _tempData;

    public HomeControllerTests()
    {
        _tempData = GetTempData();

        _controller = CreateController(_mockRepo.Object, _tempData);

        SetupUrlHelper(_mockUrlHelper, _controller);
    }

    private static TempDataDictionary GetTempData()
    {
        var mockTempDataProvider = new Mock<ITempDataProvider>();

        var tempDataDictionaryFactory = new TempDataDictionaryFactory(mockTempDataProvider.Object);

        return (TempDataDictionary)tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }

    private HomeController CreateController(IInquiryAppRepository repo, ITempDataDictionary tempData)
    {
        var controller = new HomeController(repo)
        {
            TempData = tempData,
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { TraceIdentifier = "TestTraceIdentifier" }
            }
        };

        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(serviceProvider => serviceProvider.GetService(typeof(IUrlHelperFactory)))
            .Returns(_mockUrlHelperFactory.Object);

        controller.ControllerContext.HttpContext.RequestServices = mockServiceProvider.Object;

        return controller;
    }

    private void SetupUrlHelper(Mock<IUrlHelper> mockUrlHelper, Controller controller)
    {
        mockUrlHelper.Setup(urlHelper => urlHelper.Action(It.IsAny<UrlActionContext>()))
            .Returns("/Home/Index#Contact-Us");

        _mockUrlHelperFactory.Setup(urlHelperFactory => urlHelperFactory.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(mockUrlHelper.Object);

        controller.Url = mockUrlHelper.Object;
    }

    [Fact]
    public void Index_ReturnsViewWithInquiries()
    {
        // Arrange
        var inquiries = new List<InquiryModel> { new(), new() };

        _mockRepo.Setup(repo => repo.GetAllInquiries()).Returns(inquiries);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.IsAssignableFrom<IEnumerable<InquiryModel>>(viewResult.Model);
    }

    [Fact]
    public void DeleteInquiry_SuccessRedirectsToIndex()
    {
        // Arrange
        var inquiry = new InquiryModel { Inquiry_ID = 1 };

        _mockRepo.Setup(repo => repo.DeleteInquiry(inquiry)).Returns(true);

        // Act
        var result = _controller.DeleteInquiry(inquiry);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Fact]
    public void DeleteInquiry_FailureRedirectsToError()
    {
        // Arrange
        var inquiry = new InquiryModel { Inquiry_ID = 1 };

        _mockRepo.Setup(repo => repo.DeleteInquiry(inquiry)).Returns(false);

        // Act
        var result = _controller.DeleteInquiry(inquiry);

        // Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Error", redirectToActionResult.ActionName);
    }


    [Fact]
    public void Privacy_ReturnsView()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsViewWithErrorViewModel_UsingActivityId()
    {
        // Arrange
        var activity = new Activity("TestActivity").Start();

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);

        Assert.Equal(activity.Id, model.RequestId);

        activity.Stop();
    }

    [Fact]
    public void Error_ReturnsViewWithErrorViewModel_UsingTraceIdentifier()
    {
        // Arrange
        Activity.Current = null;

        _controller.ControllerContext.HttpContext.TraceIdentifier = "TestTraceIdentifier";

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);

        Assert.Equal("TestTraceIdentifier", model.RequestId);
    }
}