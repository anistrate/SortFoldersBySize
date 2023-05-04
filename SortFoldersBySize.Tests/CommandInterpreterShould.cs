using SortFoldersBySize.Models;
using SortFoldersBySize.Services;

namespace SortFoldersBySize.Tests
{
    public class CommandInterpreterShould
    {
        private ArgumentParser commandInterpreter;

        [SetUp]
        public void Setup()
        {
            commandInterpreter = new ArgumentParser();
        }

        [TestCase("c", CommandConstants.Calculate)]
        [TestCase("C", CommandConstants.Calculate)]
        [TestCase("r", CommandConstants.RemoveTags)]
        [TestCase("R", CommandConstants.RemoveTags)]
        public void InterpretCommand_validargs_returnsCommand(string commandShorthand, string command)
        {
            var args = Stubs.GetArgsCalculateCorrect(commandShorthand);
            var expectedResult = Stubs.GetCommand(command);
            var result = commandInterpreter.ParseArguments(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Value, Is.EqualTo(result.Value));
        }

        [TestCase("i")]
        [TestCase("incorrect")]
        [TestCase("testtest")]
        public void InterpretCommand_invalidargs_returnsResultFailure(string commandShorthand)
        {
            var args = Stubs.GetArgsIncorrectCommand(commandShorthand);
            var expectedResult = Stubs.GetIncorrectCommandResult(commandShorthand);
            var result = commandInterpreter.ParseArguments(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Error, Is.EqualTo(result.Error));
        }

        [TestCase("i1", "i2")]
        [TestCase("incorrectCommand1", "incorrectCommand2")]
        public void InterpretCommand_toomanyargs_returnsResultFailure(string incorrectCommand1, string incorrectCommand2)
        {
            var args = Stubs.GetArgsMoreThan2(incorrectCommand1, incorrectCommand2);
            var expectedResult = Stubs.GetIncorrectNumberOfArgumentsResult(args.Length);
            var result = commandInterpreter.ParseArguments(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Error, Is.EqualTo(result.Error));
        }

        [Test]
        public void InterpretCommand_toofewargs_returnsResultFailure()
        {
            var args = Stubs.GetArgsOnlyOne();
            var expectedResult = Stubs.GetIncorrectNumberOfArgumentsResult(args.Length);
            var result = commandInterpreter.ParseArguments(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Error, Is.EqualTo(result.Error));
        }
    }
}