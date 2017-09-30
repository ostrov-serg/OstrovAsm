
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace OstrovAsm
{
    internal static class OstrovAsmClassificationDefinition
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.digit")]
        internal static ClassificationTypeDefinition AsmDigitDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.comment")]
        internal static ClassificationTypeDefinition AsmCommentDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.label")]
        internal static ClassificationTypeDefinition AsmLabelDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.directive")]
        internal static ClassificationTypeDefinition AsmDirectiveDefinition = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("asm.string")]
		internal static ClassificationTypeDefinition AsmStringDefinition = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("asm.const")]
		internal static ClassificationTypeDefinition AsmSConstDefinition = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("asm.variable")]
		internal static ClassificationTypeDefinition AsmVariableDefinition = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("asm.type")]
		internal static ClassificationTypeDefinition AsmTypeDefinition = null;

		[Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.instruction")]
        internal static ClassificationTypeDefinition AsmInstructionDefinition = null;

		[Export(typeof(ClassificationTypeDefinition))]
		[Name("asm.instructionFpu")]
		internal static ClassificationTypeDefinition AsmInstructionFpuDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.instructionExt")]
        internal static ClassificationTypeDefinition AsmInstructionExtDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.instructionSys")]
        internal static ClassificationTypeDefinition AsmInstructionSysDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.instructionBmi")]
        internal static ClassificationTypeDefinition AsmInstructionBmiDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.registerCpu")]
        internal static ClassificationTypeDefinition AsmRegisterCpuDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.registerFpu")]
        internal static ClassificationTypeDefinition AsmRegisterFpuDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("asm.registerExt")]
        internal static ClassificationTypeDefinition AsmRegisterExtDefinition = null;
    }
}
