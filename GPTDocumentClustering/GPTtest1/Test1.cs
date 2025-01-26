//using GPTDocumentClustering;
//namespace GPTtest1
//{
//    [TestClass]
//    public sealed class Test1
//    {
//        [TestMethod]
//        public void TestMethod1()
//        {
//        }
//    }
//}
using Moq;
using Xunit;
using System;
using System.IO;
using GPTDocumentClustering;
using OpenAI.Chat;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class ProgramTests
{
    [Fact]
    public void Run_ShouldRequestMessageCountFromUser()
    {
        // Arrange
        var mockClient = new Mock<ChatClient>(MockBehavior.Strict, "gpt-4o", "fake-api-key");
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        var program = new Program(mockClient.Object);

        // Simulate user input
        var userInput = "2";
        var originalInput = Console.In;
        Console.SetIn(new StringReader(userInput));

        program.Run();

        // Assert
        Assert.Contains("Please type the number of chat messages you want:", consoleOutput.ToString());
    }

    [Fact]
    public void Run_ShouldCallCompleteChat_WhenUserInputsMessage()
    {
        // Arrange
        var mockClient = new Mock<ChatClient>(MockBehavior.Strict, "gpt-4o", "fake-api-key");
        mockClient.Setup(client => client.CompleteChat(It.IsAny<string>())).Returns(new ChatCompletion
        {
            Content = new[] { new ChatCompletionChoice { Text = "Response text" } }
        });

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Simulate user input
        var userInput = "1";
        var messageInput = "Hello, Assistant!";
        var originalInput = Console.In;
        Console.SetIn(new StringReader(userInput + Environment.NewLine + messageInput));

        // Act
        var program = new Program(mockClient.Object);
        program.Run();

        // Assert
        mockClient.Verify(client => client.CompleteChat(It.Is<string>(s => s == messageInput)), Times.Once);
        Assert.Contains("[ASSISTANT]: Response text", consoleOutput.ToString());
    }

    [Fact]
    public void Run_ShouldHandleMultipleMessages()
    {
        // Arrange
        var mockClient = new Mock<ChatClient>(MockBehavior.Strict, "gpt-4o", "fake-api-key");
        mockClient.Setup(client => client.CompleteChat(It.IsAny<string>())).Returns(new ChatCompletion
        {
            Content = new[] { new ChatCompletionChoice { Text = "Response text" } }
        });

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Simulate user input
        var userInput = "2";
        var messageInput1 = "First message!";
        var messageInput2 = "Second message!";
        var originalInput = Console.In;
        Console.SetIn(new StringReader(userInput + Environment.NewLine + messageInput1 + Environment.NewLine + messageInput2));

        // Act
        var program = new Program(mockClient.Object);
        program.Run();

        // Assert
        mockClient.Verify(client => client.CompleteChat(It.Is<string>(s => s == messageInput1)), Times.Once);
        mockClient.Verify(client => client.CompleteChat(It.Is<string>(s => s == messageInput2)), Times.Once);
        Assert.Contains("[ASSISTANT]: Response text", consoleOutput.ToString());
    }
}

