using SortFoldersBySize.Services;

namespace SortFoldersBySize.Tests
{
    public class CommandInterpreterShould
    {
        private CommandInterpreter commandInterpreter;

        [SetUp]
        public void Setup()
        {
            commandInterpreter = new CommandInterpreter();
        }

        [Test]
        public void InterpretCommand_validargs_returnsCalculateCommand()
        {
            var args = Stubs.GetArgsCalculateCorrect();
            var expectedResult = Stubs.GetCalculateCommand();
            var result = commandInterpreter.InterpretCommand(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Value, Is.EqualTo(result.Value));
        }

        [Test]
        public void InterpretCommand_ValidArgsUpperCase_ReturnsCalculateCommand()
        {
            var args = Stubs.GetArgsCalculateUppercaseCorrect();
            var expectedResult = Stubs.GetCalculateCommand();
            var result = commandInterpreter.InterpretCommand(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Value, Is.EqualTo(result.Value));
        }


        [Test]
        public void InterpretCommand_validargs_returnsRemoveCommand()
        {
            var args = Stubs.GetArgsRemoveCorrect();
            var expectedResult = Stubs.GetRemoveCommand();
            var result = commandInterpreter.InterpretCommand(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Value, Is.EqualTo(result.Value));
        }

        [Test]
        public void InterpretCommand_invalidargs_returnsResultFailure()
        {
            var args = Stubs.GetArgsIncorrectCommand();
            var expectedResult = Stubs.GetIncorrectCommandResult();
            var result = commandInterpreter.InterpretCommand(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Error, Is.EqualTo(result.Error));
        }

        [Test]
        public void InterpretCommand_toomanyargs_returnsResultFailure()
        {
            var args = Stubs.GetArgsMoreThan2();
            var expectedResult = Stubs.GetIncorrectNumberOfArgumentsResult(args.Length);
            var result = commandInterpreter.InterpretCommand(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Error, Is.EqualTo(result.Error));
        }

        [Test]
        public void InterpretCommand_toofewargs_returnsResultFailure()
        {
            var args = Stubs.GetArgsOnlyOne();
            var expectedResult = Stubs.GetIncorrectNumberOfArgumentsResult(args.Length);
            var result = commandInterpreter.InterpretCommand(args);

            Assert.That(expectedResult.IsSuccess, Is.EqualTo(result.IsSuccess));
            Assert.That(expectedResult.Error, Is.EqualTo(result.Error));
        }
    }
}