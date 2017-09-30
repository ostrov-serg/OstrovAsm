
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace OstrovAsm
{
    #region Format definition
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.digit")]
    [Name("asm.digit")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorDigitFormat : ClassificationFormatDefinition
    {
        public AsmEditorDigitFormat()
        {
            this.DisplayName = "Ассемблер - числа";
            this.ForegroundColor = Color.FromRgb(255, 255, 0);
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.comment")]
    [Name("asm.comment")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorCommentFormat : ClassificationFormatDefinition
    {
        public AsmEditorCommentFormat()
        {
            this.DisplayName = "Ассемблер - комментарии";
            this.ForegroundColor = Colors.LightGreen;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.label")]
    [Name("asm.label")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorLabelFormat : ClassificationFormatDefinition
    {
        public AsmEditorLabelFormat()
        {
            this.DisplayName = "Ассемблер - метки";
            this.IsBold = true;
            this.ForegroundColor = Colors.LightGray;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.string")]
    [Name("asm.string")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorStringFormat : ClassificationFormatDefinition
    {
        public AsmEditorStringFormat()
        {
            this.DisplayName = "Ассемблер - строки";
            this.ForegroundColor = Colors.Magenta;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "asm.directive")]
	[Name("asm.directive")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class AsmEditorDirectiveFormat : ClassificationFormatDefinition
	{
		public AsmEditorDirectiveFormat()
		{
			this.DisplayName = "Ассемблер - директивы";
			this.ForegroundColor = Colors.BlueViolet;
			this.BackgroundColor = Color.FromRgb(40, 40, 40);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "asm.const")]
	[Name("asm.const")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class AsmEditorConstFormat : ClassificationFormatDefinition
	{
		public AsmEditorConstFormat()
		{
			this.DisplayName = "Ассемблер - константы";
			this.ForegroundColor = Colors.BlueViolet;
			this.BackgroundColor = Color.FromRgb(40, 40, 40);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "asm.variable")]
	[Name("asm.variable")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class AsmEditorVariableFormat : ClassificationFormatDefinition
	{
		public AsmEditorVariableFormat()
		{
			this.DisplayName = "Ассемблер - переменные";
			this.ForegroundColor = Colors.BlueViolet;
			this.BackgroundColor = Color.FromRgb(40, 40, 40);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "asm.type")]
	[Name("asm.type")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class AsmEditorTypeFormat : ClassificationFormatDefinition
	{
		public AsmEditorTypeFormat()
		{
			this.DisplayName = "Ассемблер - типы";
			this.ForegroundColor = Colors.White;
			this.BackgroundColor = Color.FromRgb(40, 40, 40);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "asm.instruction")]
	[Name("asm.instruction")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class AsmEditorInstructionFormat : ClassificationFormatDefinition
	{
		public AsmEditorInstructionFormat()
		{
			this.DisplayName = "Ассемблер - инструкции cpu";
			this.ForegroundColor = Colors.LightBlue;
			this.BackgroundColor = Color.FromRgb(40, 40, 40);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
	[ClassificationType(ClassificationTypeNames = "asm.instructionFpu")]
	[Name("asm.instructionFpu")]
	[UserVisible(true)]
	[Order(Before = Priority.Default)]
	internal sealed class AsmEditorInstructionFpuFormat : ClassificationFormatDefinition
	{
		public AsmEditorInstructionFpuFormat()
		{
			this.DisplayName = "Ассемблер - инструкции fpu";
			this.ForegroundColor = Colors.LightCoral;
			this.BackgroundColor = Color.FromRgb(40, 40, 40);
		}
	}

	[Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.instructionExt")]
    [Name("asm.instructionExt")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorInstructionExtFormat : ClassificationFormatDefinition
    {
        public AsmEditorInstructionExtFormat()
        {
            this.DisplayName = "Ассемблер - инструкции simd";
            this.ForegroundColor = Colors.Aquamarine;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.instructionBmi")]
    [Name("asm.instructionBmi")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorInstructionBmiFormat : ClassificationFormatDefinition
    {
        public AsmEditorInstructionBmiFormat()
        {
            this.DisplayName = "Ассемблер - инструкции bmi";
            this.ForegroundColor = Colors.AliceBlue;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.instructionSys")]
    [Name("asm.instructionSys")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorInstructionSysFormat : ClassificationFormatDefinition
    {
        public AsmEditorInstructionSysFormat()
        {
            this.DisplayName = "Ассемблер - системные инструкции";
            this.ForegroundColor = Colors.BurlyWood;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.registerCpu")]
    [Name("asm.registerCpu")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorRegisterCpuFormat : ClassificationFormatDefinition
    {
        public AsmEditorRegisterCpuFormat()
        {
            this.DisplayName = "Ассемблер - регистры cpu";
            this.ForegroundColor = Colors.Red;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.registerFpu")]
    [Name("asm.registerFpu")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorRegisterFpuFormat : ClassificationFormatDefinition
    {
        public AsmEditorRegisterFpuFormat()
        {
            this.DisplayName = "Ассемблер - регистры fpu";
            this.ForegroundColor = Colors.Red;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "asm.registerExt")]
    [Name("asm.registerExt")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class AsmEditorRegisterExtFormat : ClassificationFormatDefinition
    {
        public AsmEditorRegisterExtFormat()
        {
            this.DisplayName = "Ассемблер - регистры simd";
            this.ForegroundColor = Colors.Red;
            this.BackgroundColor = Color.FromRgb(40, 40, 40);
            this.IsBold = true;
        }
    }
    #endregion
}
