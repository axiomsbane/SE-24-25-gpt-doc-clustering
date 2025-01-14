//namespace UnittestGPT
//{
//    public class UnitTest1
//    {
//        [Fact]
//        public void Test1()
//        {

//        }
//    }
//}
using Moq;
using System;
using System.IO;
using Xunit;
using GPTDocumentClustering;
using OpenAI.Chat;

namespace GPTDocumentClustering.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Run_ShouldCallCompleteChat_WhenUserInputsMessage()
        {
            // Arrange
            var mockClient = new Mock<ChatClient>("gpt-4o", "fake-api-key");
            mockClient.Setup(client => client.CompleteChat(It.IsAny<string>()))
                      .ReturnsAsync(new ChatCompletion
                      {
                          Content = new[] { new ChatCompletionChoice { Text = "Response text" } }
                      });

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var userInput = "1"; // User enters 1 message
            var messageInput = "Hello, Assistant!";
            var originalInput = Console.In;
            Console.SetIn(new StringReader(userInput + Environment.NewLine + messageInput));

            var program = new Program(mockClient.Object);

            // Act
            program.Run();

            // Assert: Check that CompleteChat was called with the correct input.
            mockClient.Verify(client => client.CompleteChat(It.Is<string>(s => s == messageInput)), Times.Once);

            // Assert: Check that the assistant's response is printed.
            Assert.Contains("[ASSISTANT]: Response text", consoleOutput.ToString());
        }
    }
}

