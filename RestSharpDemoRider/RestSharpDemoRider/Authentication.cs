using System;
using System.Threading.Tasks;
using FluentAssertions;
using GraphQLProductApp.Controllers;
using GraphQLProductApp.Data;
using Newtonsoft.Json.Linq;
using RestSharp;
using Xunit;

namespace RestSharpDemoRider;

public class Authentication
{

    private RestClientOptions _restClientOptions;
    public Authentication()
    {
        _restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri("https://localhost:44330/"),
            RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
        };  
    }
    
    [Fact]
    public async Task GetWithQueryParameterTest()
    {
        //Rest Client
        var client = new RestClient(_restClientOptions);
        //Rest Request
        var authRequest = new RestRequest("api/Authenticate/Login");
        
        // //Anonymous object being passed as body in request
        // request.AddJsonBody(new
        // {
        //     username = "KK",
        //     password = "123456"
        // });
        //
             
        //Typed object being passed as body in request
        authRequest.AddJsonBody(new LoginModel
        {
            UserName = "KK",
            Password = "123456"
        });

        //Perform GET operation
        var authResponse = client.PostAsync(authRequest).Result.Content;

        //Token from JSON object
        var token = JObject.Parse(authResponse)["token"];
        //Rest Request
        var productGetRequest = new RestRequest("Product/GetProductById/1");
        productGetRequest.AddHeader("Authorization", $"Bearer {token.ToString()}");
        
        //Perform GET operation
        var productResponse = await client.GetAsync<Product>(productGetRequest);
        //Assert
        productResponse?.Name.Should().Be("Keyboard");
    }
}