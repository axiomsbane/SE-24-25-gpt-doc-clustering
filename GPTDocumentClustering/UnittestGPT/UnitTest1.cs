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

namespace UnittestGPT
{
    public class ProgramTests
    {
        // Test when the user is prompted for the number of messages.
        [Fact]
        public void Run_ShouldRequestNumberOfMessages()
        {
            // Arrange
            var mockClient = new Mock<ChatClient>("gpt-4o", "fake-api-key");
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var userInput = "2"; // Simulate user input for 2 messages.
            var originalInput = Console.In;
            Console.SetIn(new StringReader(userInput));

            var program = new Program(mockClient.Object);

            // Act
            program.Run();

            // Assert
            Assert.Contains("Please type the number of chat messages you want:", consoleOutput.ToString());
        }

        // Test that checks if CompleteChat is called and its result is printed.
        [Fact]
        public void Run_ShouldCallCompleteChat_WhenUserInputsMessage()
        {
            // Arrange
            var mockClient = new Mock<ChatClient>("gpt-4o", "fake-api-key");
            mockClient.Setup(client => client.CompleteChat(It.IsAny<string>()))
                      .Returns(new ChatCompletion
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

        public ChatCompletionChoice[] GetChatCompletionChoices()
        {
            return new[] { new ChatCompletionChoice { Text = "Response text" } };
        }

        // Test that checks the handling of multiple messages.
        [Fact]
        public void Run_ShouldHandleMultipleMessages(ChatCompletionChoice[] chatCompletionChoices)
        {
            // Arrange
            var mockClient = new Mock<ChatClient>("gpt-4o", "fake-api-key");
            mockClient.Setup(client => client.CompleteChat(It.IsAny<string>()))
                      .Returns(new ChatCompletion
                      {
                          Content = chatCompletionChoices
                      });

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var userInput = "2"; // User inputs 2 messages
            var messageInput1 = "First message!";
            var messageInput2 = "Second message!";
            var originalInput = Console.In;
            Console.SetIn(new StringReader(userInput + Environment.NewLine + messageInput1 + Environment.NewLine + messageInput2));

            var program = new Program(mockClient.Object);

            // Act
            program.Run();

            // Assert that CompleteChat was called twice (once for each message)
            mockClient.Verify(client => client.CompleteChat(It.Is<string>(s => s == messageInput1)), Times.Once);
            mockClient.Verify(client => client.CompleteChat(It.Is<string>(s => s == messageInput2)), Times.Once);

            // Assert that the assistant's response is printed
            Assert.Contains("[ASSISTANT]: Response text", consoleOutput.ToString());
        }

        // Test that handles invalid input for the number of messages.
        [Fact]
        public void Run_ShouldHandleInvalidNumberOfMessages()
        {
            // Arrange
            var mockClient = new Mock<ChatClient>("gpt-4o", "fake-api-key");
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            var invalidUserInput = "invalid"; // Simulate invalid input for the number of messages.
            var originalInput = Console.In;
            Console.SetIn(new StringReader(invalidUserInput));

            var program = new Program(mockClient.Object);

            // Act
            program.Run();

            // Assert: Check that the program doesn't crash and prints an error message for invalid input
            Assert.Contains("Invalid input.", consoleOutput.ToString());
        }
    }
}
