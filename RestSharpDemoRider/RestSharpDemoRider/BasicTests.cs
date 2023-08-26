using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using GraphQLProductApp.Controllers;
using GraphQLProductApp.Data;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xunit;

namespace RestSharpDemoRider;

public class BasicTests
{
    private readonly RestClientOptions _restClientOptions;

    public BasicTests()
    {
        _restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri("https://localhost:44330/"),
            RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
        };
    }


    [Fact]
    public async Task GetOperationTest()
    {
        //Rest Client
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var request = new RestRequest("Product/GetProductById/1");
        request.AddHeader("Authorization", $"Bearer {GetToken()}");
        //Perform GET operation
        var response = await client.GetAsync<Product>(request);
        //Assert
        response?.Name.Should().Be("Keyboard");
    }

    [Fact]
    public async Task GetWithQuerySegmentTest()
    {
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var request = new RestRequest("Product/GetProductById/{id}");
        request.AddHeader("Authorization", $"Bearer {GetToken()}");
        request.AddUrlSegment("id", 2);
        //Perform GET operation
        var response = await client.GetAsync<Product>(request);
        //Assert
        response?.Price.Should().Be(400);
    }

    [Fact]
    public async Task GetWithQueryParameterTest()
    {
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var request = new RestRequest("Product/GetProductByIdAndName");
        request.AddHeader("Authorization", $"Bearer {GetToken()}");
        request.AddQueryParameter("id", 2);
        request.AddQueryParameter("name", "Monitor");
        //Perform GET operation
        var response = await client.GetAsync<Product>(request);
        //Assert
        response?.Price.Should().Be(400);
    }


    [Fact]
    public async Task PostProductTest()
    {
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var request = new RestRequest("Product/Create");
        request.AddHeader("Authorization", $"Bearer {GetToken()}");
        request.AddJsonBody(new Product
        {
            Name = "Cabinet",
            Description = "Gaming Cabinet",
            Price = 300,
            ProductType = ProductType.PERIPHARALS
        });
        //Perform POST operation
        var response = await client.PostAsync<Product>(request);
        //Assert
        response?.Price.Should().Be(300);
    }


    [Fact]
    public async Task FileUploadTest()
    {
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var request = new RestRequest("Product", Method.Post);
        request.AddHeader("Authorization", $"Bearer {GetToken()}");
        request.AddFile("myFile", @"C:\Users\Ayman.Hesham\Downloads\CI-CD output.PNG", "multipart/form-data");
        //Perform Execute operation
        var response = await client.ExecuteAsync(request);
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }


    private string GetToken()
    {
        //Rest Client
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var authRequest = new RestRequest("api/Authenticate/Login");
       //Typed object being passed as body in request
        authRequest.AddJsonBody(new LoginModel
        {
            UserName = "KK",
            Password = "123456"
        });

        //Perform GET operation
        var authResponse = client.PostAsync(authRequest).Result.Content;

        //Token from JSON object
        return JObject.Parse(authResponse)["token"].ToString();
    }
}