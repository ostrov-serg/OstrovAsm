
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.IO;

namespace OstrovAsm
{
    #region Provider definition

    [Export(typeof(IClassifierProvider))]
    [ContentType("asm")]
    internal class OstrovAsmProvider : IClassifierProvider
    {
        [Export]
        [Name("asm")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition AsmContentType = null;

        [Export]
        [FileExtension(".asm")]
        [ContentType("asm")]
        internal static FileExtensionToContentTypeDefinition AsmFileType = null;

        [Export]
        [FileExtension(".inc")]
        [ContentType("asm")]
        internal static FileExtensionToContentTypeDefinition IncFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
			OstrovAsm _ostrov = new OstrovAsm(ClassificationRegistry);
			_ostrov.init(buffer.CurrentSnapshot);
            return buffer.Properties.GetOrCreateSingletonProperty<OstrovAsm>(delegate { return _ostrov; });
        }
    }
    #endregion //provider def

    #region Classifier
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    class OstrovAsm : IClassifier
    {
        IClassificationTypeRegistryService classType;

        internal OstrovAsm(IClassificationTypeRegistryService registry)
        {
            classType = registry;
        }

		public void init(ITextSnapshot snapshot)
		{
			// FileInfo fi1 = new FileInfo("e:\\1.txt");
			//StreamWriter sw = fi1.CreateText();
			if (snapshot != null && snapshot.Length != 0)
			{
				// sw.WriteLine(snapshot.LineCount);
				int count = snapshot.LineCount - 1;
                for (int i = 0; i < count; i++)
				{
					ITextSnapshotLine line = snapshot.GetLineFromLineNumber(i);
					// обрабатываем текущую строку текста
					// берем текст строки
					string text = line.GetText();
					int pos = 0, pos1, len;
						// идентифицируем лексемы
						// 1. строка
						// 2. комментарии
						len = text.IndexOf(';');
						if (len >= 0) continue;
						len = text.Length;
						bool is_begin = true;
						string word = null;
						while (pos < len)
						{
							// строка
							if (text[pos] == '\"' || text[pos] == '\'') break;
							pos1 = text.IndexOfAny(chars, pos);
							if (pos1 < 0) pos1 = len;
							if (pos != pos1)
							{
								if(is_begin && pos1 < len)
								{
									word = text.Substring(pos, pos1 - pos);
									if (text[pos1] == ':') { lex_label.Add(word); break; }
								}
								else if(word != null)
								{
									if (is_lexem(text, pos, pos1, lex_directives1))
									{
										if (!is_lexem_list(word, 0, word.Length, lex_var)) lex_var.Add(word);
									}
									else if (is_lexem(text, pos, pos1, lex_directives2))
									{
										if (!is_lexem_list(word, 0, word.Length, lex_type)) lex_type.Add(word);
									}
									else
									{
										if (text[pos] == '=' && !is_lexem_list(word, 0, word.Length, lex_const)) lex_const.Add(word);
									}
									break;
								}
								is_begin = false;
							}
							pos = pos1 + 1;
						}
				}
			}
			is_first = false;
		}
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;

			//if (is_first) init(snapshot);
	
            List<ClassificationSpan> result = new List<ClassificationSpan>();

			if(span.Length != 0)
			{
				int startno = span.Start.GetContainingLine().LineNumber;
                int endno = (span.End - 1).GetContainingLine().LineNumber;

                for (int i = startno; i <= endno; i++)
                {
                    ITextSnapshotLine line = snapshot.GetLineFromLineNumber(i);
                    // обрабатываем текущую строку текста
                    // берем текст строки
                    string text = line.GetText();
                    int pos = 0, pos1, len;
                    // идентифицируем лексемы
                    // 1. строка
                    // 2. комментарии
                    len = text.IndexOf(';');
                    if (len >= 0)
                    {
                        result.Add(new ClassificationSpan(new SnapshotSpan(line.Snapshot, new Span(line.Start + len, line.Length - len)), classType.GetClassificationType("asm.comment")));
                    }
                    else
                    {
                        len = text.Length;
                    }
                    // если в начале строки и в конце слова : - то метка
                    // если в начале строки и перед proc - то имя процедуры
                    // если в начале строки и перед dd, db, dw, dq - то имя переменной
					// если = - то слово в начале константа
                    bool is_begin = true;
					string word = null;
                    while (pos < len)
                    {
                        // строка
                        if (text[pos] == '\"' || text[pos] == '\'')
                        {
                            pos1 = pos + 1;
                            bool is_slash = false;
                            while (pos1 < len)
                            {
                                if (text[pos1] == '\\') { is_slash = true; pos1++; continue; }
                                else if (is_slash == false && (text[pos1] == '\'' || text[pos1] == '\"')) { pos1++; break; }
                                pos1++;
                                is_slash = false;
                            }
                            result.Add(new ClassificationSpan(new SnapshotSpan(line.Snapshot, new Span(line.Start + pos, pos1 - pos)), classType.GetClassificationType("asm.string")));
                            pos = pos1;
                            continue;
                        }
                        IClassificationType type = null;

                        pos1 = text.IndexOfAny(chars, pos);
                        if (pos1 < 0) pos1 = len;
                        if (pos != pos1)
                        {
                            int offs = ((text[pos] == 'v' || text[pos] == 'V') ? 1 : 0);
                            if (text[pos] >= '0' && text[pos] <= '9') type = classType.GetClassificationType("asm.digit");
                            else if (is_lexem(text, pos, pos1, lex_instructions)) type = classType.GetClassificationType("asm.instruction");
                            else if (is_lexem(text, pos, pos1, lex_registersCpu)) type = classType.GetClassificationType("asm.registerCpu");
                            else if (is_lexem(text, pos, pos1, lex_instructionsFpu)) type = classType.GetClassificationType("asm.instructionFpu");
                            else if (is_lexem(text, pos + offs, pos1, lex_instructionsExt)) type = classType.GetClassificationType("asm.instructionExt");
                            else if (is_lexem(text, pos, pos1, lex_directives1))
                            {
                                if (word != null && !is_lexem_list(word, 0, word.Length, lex_var)) { lex_var.Add(word); word = null; }
                                type = classType.GetClassificationType("asm.directive");
                            }
                            else if (is_lexem(text, pos, pos1, lex_directives2))
                            {
                                if (word != null && !is_lexem_list(word, 0, word.Length, lex_type)) { lex_type.Add(word); word = null; }
                                type = classType.GetClassificationType("asm.directive");
                            }
                            else if (is_lexem(text, pos, pos1, lex_directives3)) type = classType.GetClassificationType("asm.directive");
                            else if (is_lexem(text, pos, pos1, lex_registersExt)) type = classType.GetClassificationType("asm.registerExt");
                            else if (is_lexem(text, pos, pos1, lex_registersFpu)) type = classType.GetClassificationType("asm.registerFpu");
                            else if (is_lexem_list(text, pos, pos1, lex_const)) type = classType.GetClassificationType("asm.const");
                            else if (is_lexem_list(text, pos, pos1, lex_var)) type = classType.GetClassificationType("asm.variable");
                            else if (is_lexem_list(text, pos, pos1, lex_type)) type = classType.GetClassificationType("asm.type");
                            else if (is_lexem_list(text, pos, pos1, lex_label)) type = classType.GetClassificationType("asm.label");
                            else
                            {
                                if (is_begin && pos1 < len)
                                {
                                    word = text.Substring(pos, pos1 - pos);
                                    if (text[pos1] == ':') { lex_label.Add(word); word = null; type = classType.GetClassificationType("asm.label"); }
                                }
                                else if (word != null && text[pos] == '=' && !is_lexem_list(word, 0, word.Length, lex_const)) { lex_const.Add(word); word = null; }
                            }
                            if (type != null)
                            {
                                result.Add(new ClassificationSpan(new SnapshotSpan(line.Snapshot, new Span(line.Start + pos, pos1 - pos)), type));
                            }
							is_begin = false;
						}
						pos = pos1 + 1;
                    }
                }
            }
            return result;
        }

#pragma warning disable 67
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

        private static readonly char[] chars = { ' ', '\t', ',', '[', '+', '-', ';', ']', '*', '(', ')', ':', '<', '>', '}', '{' };
		private List<string> lex_const = new List<string>();
		private List<string> lex_var = new List<string>();
		private List<string> lex_type = new List<string>();
		private List<string> lex_label = new List<string>();
		bool is_first = true;
		private bool is_lexem(string text, int pos, int pos1, string[] arr)
		{
			int len = pos1 - pos;
			for (int i = 0; i < arr.Length; i++)
			{
				if ((string.Compare(text, pos, arr[i], 0, len, true) == 0) && len == arr[i].Length)
					return true;
			}
			return false;
		}

		private bool is_lexem_list(string text, int pos, int pos1, List<string> arr)
		{
			int len = pos1 - pos;
			for (int i = 0; i < arr.Count; i++)
			{
				if ((string.Compare(text, pos, arr[i], 0, len, true) == 0) && len == arr[i].Length)
					return true;
			}
			return false;
		}

		private static readonly string[] lex_instructions =
        {
            "aaa", "aad", "aam", "aas", "adc", "add", "and", "andn", "arpl",
            "bextr", "blsi", "blsmsk", "blsr", "bswap", "bt", "btc", "btr", "bts", "bzhi",
            "call", "cbw", "cwde", "cdqe", "clc", "cld", "clflush", "cli", "clts", "cmc", "cmp", "cmpxchg", "cmpxchg8b", "cmpxchg16b", "cpuid", "crc32", "cwd", "cdq", "cqo", "cmps", "cmpsb", "cmpsw", "cmpsd", "cmpsq",
            "cmova", "cmovae", "cmovb", "cmovbe", "cmovc", "cmove", "cmovg", "cmovge", "cmovl", "cmovle", "cmovna", "cmovnae", "cmovnb", "cmovnbe", "cmovnc", "cmovne", "cmovng", "cmovnge", "cmovnl", "cmovnle", "cmovns", "cmovnz", "cmovs", "cmovz",
            "daa", "das", "dec", "div",
            "enter",
            "hlt",
            "idiv", "imul", "inc", "in", "int","into","int 3","invd","invlpg","invpcid","iret", "iretd",
            "jmp",
            "ja", "jae", "jb", "jbe", "jc", "jcxz", "jecxz", "jrcxz", "je", "jg", "jge", "jl", "jle", "jna", "jnae", "jnb", "jnbe", "jnc", "jne", "jng", "jnge", "jns", "jnz", "js", "jz",
            "lahf", "lar", "lds", "lss", "les", "lfs", "lgs", "lea", "leave", "lfence", "lgdt", "ligdt", "lldt", "lmsw", "lock", "lods", "lodsb", "lodsw", "lodsd", "lodsq", "loop", "loopz", "loopnz", "lsl", "ltr", "lzcnt",
            "mfence", "monitor", "mov", "movbe","movs","movsb", "movsw", "movsd", "movsq", "movsx", "movsxd", "movzx", "mul","mwait",
            "neg", "nop", "not",
            "or", "out",
            "pause", "pdep", "pext", "pop", "popa", "popad", "popcnt", "popf", "popfd", "popfq", "prefetcht0", "prefetcht1", "prefetcht2", "prefetchnta", "prefetchw", "push", "pusha", "pushad", "pushf", "pushfd", "pushfq",
            "rcl", "rcr", "rol", "ror", "rdmsr", "rdpmc", "rdrand", "rdtsc", "rdtscp", "rep", "repz", "repnz", "ret", "rorx", "rsm", "retn",
            "sahf", "sal", "sar", "shl", "shr", "sarx", "shlx", "shrx", "sbb", "scas", "scasb", "scasw", "scasd", "scasq",
            "seta", "setae", "setb", "setbe", "setc", "sete", "setg", "setge", "setl", "setle", "setna", "setnae", "setnb", "setnbe", "setnc", "setne", "setng", "setnge", "setnl", "setnle", "setns", "setnz", "sets", "setz",
            "sfence", "sgdt", "shld", "shrd", "sidt", "sldt", "smsw", "stc", "std", "sti", "stos","stosb", "stosw", "stosd", "stosq", "sub", "swapgs", "syscall", "sysenter", "sysexit", "sysret",
            "test", "tzcnt",
            "wbinvd", "wrfsbase", "wrgsbase", "wrmsr",
            "xacquire", "xrelease", "xabort", "xadd", "xbegin", "xchg", "xend", "xgetbv", "xlat", "xlatb", "xor", "xrstor", "srstors", "xsave", "xsavec", "xsaveopt", "xsaves", "xsetbv", "xtest"
        };
		private static readonly string[] lex_instructionsFpu =
		{
			"f2xm1", "fabs", "fadd", "fiadd", "faddp", "fbld", "fbstp", "fchs", "fclex", "fnclex", "fcom", "fcomp", "fcompp", "fcomi", "fcomip", "fucomi", "fucomip", "fcos", "fdecstp", "fdiv", "fidiv", "fdivp", "fdivr", "fidivr", "fdivrp",
			"ffree", "ficom", "ficomp", "fild", "fincstp", "finit", "fninit", "fist", "fistp", "fisttp", "fld", "fld1", "fldl2t", "fldl2e", "fldpi", "fldlg2", "fldln2", "fldz", "fldcw", "fldenv", "fmul", "fimul", "fmulp", "fpatan", "fprem", "fprem1",
			"fptan", "frndint", "frstor", "fsave", "fnsave", "fscale", "fsin", "fsincos", "fsqrt", "fst", "fstp", "fstcw", "fstenv", "fstsw", "fsub", "fisub", "fsubp", "fsubr", "fisubr", "fsubrp", "ftst", "fucomp", "fucomp", "fucompp", "fxam", "fxch",
			"fxrstor", "fxsave", "fxtract", "fyl2x", "fyl2xp1"
		};
        private static readonly string[] lex_instructionsExt =
		{
			"addpd", "addps", "addsd", "addss", "vaddpd", "vaddps", "vaddsd", "vaddss", "addsubpd", "addsubps", "vaddsubpd", "vaddsubps", "aesdec", "vaesdec", "aesdeclast", "vaesdeclast",
            "aesenc", "vaesenc", "aesenclast", "vaesenclast", "aesimc", "vaesimc", "aeskeygenassist", "vaeskeygenassist", "andpd", "andps", "vandpd", "vandps", "andnpd", "andnps", "vandnpd", "vandnps",
            "blendpd", "blendps", "vblendpd", "vblendps", "blendvpd", "blendvps", "vblendbvpd", "vblendvps",
            "cmppd", "cmpps", "cmpsd", "cmpss", "vcmppd", "vcmpps", "vcmpsd", "vcmpss", "comisd", "comiss", "vcomisd", "vcomiss",
            "cvtdq2ps", "cvtdq2pd", "cvtpd2dq", "cvtpd2pi", "cvtpd2ps", "cvtpi2pd", "cvtpi2ps", "cvtps2dq", "cvtps2pd", "cvtps2pi", "cvtsd2si", "cvtsd2ss",
            "cvtsi2sd", "cvtsi2ss", "cvtss2sd", "cvtss2si", "cvttpd2dq", "cvttpd2pi", "cvttps2dq", "cvttps2pi", "cvttsd2si", "cvttss2si",
            "vcvtdq2ps", "vcvtdq2pd", "vcvtpd2dq", "vcvtpd2pi", "vcvtpd2ps", "vcvtpi2pd", "vcvtpi2ps", "vcvtps2dq", "vcvtps2pd", "vcvtps2pi", "vcvtsd2si", "vcvtsd2ss",
            "vcvtsi2sd", "vcvtsi2ss", "vcvtss2sd", "vcvtss2si", "vcvttpd2dq", "vcvttpd2pi", "vcvttps2dq", "vcvttps2pi", "vcvttsd2si", "vcvttss2si",
            "divpd", "divps", "divsd", "divss", "vdivpd", "vdivps", "vdivsd", "vdivss", "dppd", "dpps", "vdppd", "vdpps",
            "emms", "extractps", "vextractps", "haddpd", "haddps", "vhaddpd", "vhaddps", "lddqu", "vlddqu",
            "maskmovdqu", "vmaskmovdqu", "maskmovq", "maxpd", "maxps", "maxsd", "maxss", "vmaxpd", "vmaxps", "vmaxsd", "vmaxss", "minpd", "minps", "minsd", "minss", "vminpd", "vminps", "vminsd", "vminss",
            "movapd", "movaps", "vmovapd", "vmovaps", "movd", "movq", "vmovd", "vmovq", "movddup", "vmovddup", "movdqa", "movdqu", "movdq2q", "vmovdqa", "vmovdqu", "movhlps", "movhpd", "vmovhlps", "vmovhpd",
            "movhps", "movlhps", "vmovhps", "vmovlhps", "movlpd", "movlps", "vmovlpd", "vmovlps", "movmskpd", "movmskps", "vmovmskpd", "vmosmskps", "movntdqa", "vmovntdqa",
            "movntdq", "vmovntdq", "movnti", "movntpd", "movntps", "vmovntpd", "vmovntps", "movntq", "movq2dq", "movsd", "vmovsd", "movshdup", "movsldup", "vmovshdup", "vmovsldup", "movss", "vmovss", "movsd", "vmovsd", "movupd", "vmovupd",
            "movups", "vmovups", "mpsadbw", "vmpsadbw", "mulpd", "mulps", "mulsd", "mulss", "vmulpd", "vmulps", "vmulsd", "vmulss",
            "orpd", "orps", "vorpd", "vorps", "pabsb", "pabsw", "pabsd", "vpabsb", "vpabsw", "vpabsd", "packsswb", "packssdw", "vpacksswb", "vpackssdw", "packuswb", "packusdw", "vpackuswb", "vpackusdw",
            "paddb", "paddw", "paddd", "vpaddb", "vpaddw", "vpaddd", "paddq", "vpaddq", "paddsb", "paddsw", "vpaddsb", "vpaddsw", "paddusb", "paddusw", "vpaddusb", "vpaddusw", "palignr", "vpalignr", "pand", "pandn",
            "vpand", "vpandn", "pagvb", "pagvw", "vpagvb", "vpagvw", "pblendvb", "pblendw", "vpblendvb", "vpblendw", "pclmulqdq", "vpclmulqdq", "pcmpeqb", "pcmpeqw", "pcmpeqd", "pcmpeqq",
            "vcmpeqb", "vcmpeqw", "vcmpeqd", "vcmpeqq", "pcmpestri", "pcmpestrm", "vpcmpestri", "vpcmpestrm", "pcmpgtb", "pcmpgtw", "pcmpgtd", "pcmpgtq", "vpcmpgtb", "vpcmpgtw", "vpcmpgtd", "vpcmpgtq",
            "pcmpistri", "pcmpistrm", "vpcmpistri", "vpcmpistrm", "pextrb", "pextrw", "pextrd", "pextrq", "vpextrb", "vpextrw", "vpextrd", "vpextrq", "phaddw", "phaddd", "vphaddw", "vphaddd", "phaddsw", "vphaddsw",
            "phminposuw", "vphminposuw", "phsubw", "phsubd", "phsubsw", "vphsubw", "vphsubd", "vphsubsw", "pinsrb", "pinsrw", "pinsrd", "pinsrq", "vpinsrb", "vpinsrw", "vpinsrd", "vpinsrq",
            "pmaddubsw", "vpmaddubsw", "pmaddwd", "vpmaddwd",
            "pmaxsb", "pmaxsw", "pmaxsd", "pmaxub", "pmaxuw", "pmaxud", "pminsb", "pminsw", "pminsd", "pminub", "pminuw", "pminud",
            "vpmaxsb", "vpmaxsw", "vpmaxsd", "vpmaxub", "vpmaxuw", "vpmaxud", "vpminsb", "vpminsw", "vpminsd", "vpminub", "vpminuw", "vpminud", "pmovmskb", "vpmovmskb",
            "pmovsxbw","pmovsxbd","pmovsxbq","pmovsxwd","pmovsxwq","pmovsxdq", "pmovzxbw","pmovzxbd","pmovzxbq","pmovzxwd","pmovzxwq","pmovzxdq",
            "vpmovsxbw","vpmovsxbd","vpmovsxbq","vpmovsxwd","vpmovsxwq","vpmovsxdq", "vpmovzxbw", "vpmovzxbd", "vpmovzxbq", "vpmovzxwd","vpmovzxwq","vpmovzxdq",
            "pmuldq", "vpmuldq", "pmulhrsw", "vpmulhrsw", "pmulhw", "pmulhuw", "vpmulhw", "vpmulhuw", "pmulld", "pmullw", "vpmulld", "vpmullw", "pmuludq", "vpmuludq",
            "por", "vpor",
            "psadbw", "vpsadbw", "pshufb", "vpshufb",
            "pshufd", "vpshufd", "pshufhw", "pshuflw", "vpshufhw", "vpshuflw", "pshufw",
            "psignb", "psignw", "psignd", "vpsignb", "vpsignw", "vpsignd", "pslldq", "vpslldq", "psllw", "pslld", "psllq", "vpsllw", "vpslld", "vpsllq", "psraw", "psrad", "vpsraw", "vpsrad", "psrldq", "vpsrldq",
            "psrlw", "psrld", "psrlq", "vpsrlw", "vpsrld", "vpsrlq", "psubb", "psubw", "psubd", "psubq", "vpsubb", "vpsubw", "vpsubd", "vpsubq", "psubsb", "psubsw", "psubusb", "psubusw", "vpsubsb", "vpsubsw", "vpsubusb", "vpsubusw",
            "ptest", "vptest",
            "punpckhbw", "punpckhwd", "punpckhdq", "punpckhqdq", "punpcklbw", "punpcklwd", "punpckldq", "punpcklqdq",
            "vpunpckhbw", "vpunpckhwd", "vpunpckhdq", "vpunpckhqdq", "vpunpcklbw", "vpunpcklwd", "vpunpckldq", "vpunpcklqdq",
            "pxor", "vpxor",
            "rcpps", "rcpss", "vrcpps", "vrcpss", "roundpd", "roundps", "roundsd", "roundss",  "vroundpd",  "vroundps", "vroundsd", "vroundss", "rsqrtps", "rsqrtss", "vrsqrtps", "vrsqrtss",
            "shufpd", "shufps", "vshufpd", "vshufps", "sqrtpd", "sqrtps", "sqrtsd", "sqrtss", "vsqrtpd", "vsqrtps", "vsqrtsd", "vsqrtss", "subpd", "subps", "vsubpd", "vsubps", "subsd", "subss", "vsubsd", "vsubss",
            "ucomisd", "ucomiss", "vucomisd", "vucomiss", "unpcckhpd", "unpcklpd", "unpckhps", "unpcklps", "vunpckhpd", "vunpcklpd", "vunpckhps", "vunpcklps", "xorpd", "xorps", "vxorpd", "vxorps",
            "vbroadcastss", "vbroadcastsd", "vbroadcastf128", "vcvtph2ps", "vcvtps2ph", "vextractf128", "vextracti128", "vgatherdpd", "vgatgerqpd", "vgatherdps", "vgatherqps", "vgatherdd", "vgatherqd", "vgatherdq", "vgatherqq",
            "vinsertf128", "vinserti128", "vmaskmovps", "vmaskmovpd", "vpblendd", "vpbroadcastb", "vpbroadcastw", "vpbroadcastd", "vpbroadcasrq", "vbroadcasti128", "vpermd", "vpermq", "vpermpd", "vpermps",
            "vperm2i128", "vperm2f128", "vpermilpd", "vpermilps", "vpmaskmovd", "vpmaskmovq", "vpsllvd", "vpsllvq", "vtestpd", "vtestps", "vzeroall", "vzeroupper", "vpsravd", "vpsrlvd", "vpsrlvq",
            // fma
            "vfmadd132pd", "vfmadd213pd", "vfmadd231pd", "vfmadd132ps", "vfmadd213ps", "vfmadd231ps", "vfmadd132sd", "vfmadd213sd", "vfmadd231sd", "vfmadd132ss", "vfmadd213ss", "vfmadd231ss",
            "vfnmadd132pd", "vfnmadd213pd", "vfnmadd231pd", "vfnmadd132ps", "vfnmadd213ps", "vfnmadd231ps", "vfnmadd132sd", "vfnmadd213sd", "vfnmadd231sd", "vfnmadd132ss", "vfnmadd213ss", "vfnmadd231ss",
            "vfmaddsub132pd", "vfmaddsub213pd", "vfmaddsub231pd", "vfmaddsub132ps", "vfmaddsub213ps", "vfmaddsub231ps", "vfmaddsub132sd", "vfmaddsub213sd", "vfmaddsub231sd", "vfmaddsub132ss", "vfmaddsub213ss", "vfmaddsub231ss",
            "vfnmsub132pd", "vfnmsub213pd", "vfnmsub231pd", "vfnmsub132ps", "vfnmsub213ps", "vfnmsub231ps", "vfnmsub132sd", "vfnmsub213sd", "vfnmsub231sd", "vfnmsub132ss", "vfnmsub213ss", "vfnmsub231ss",
            "vfnmsub132pd", "vfmsub213pd", "vfmsub231pd", "vfmsub132ps", "vfmsub213ps", "vfmsub231ps", "vfmsub132sd", "vfmsub213sd", "vfmsub231sd", "vfmsub132ss", "vfmsub213ss", "vfmsub231ss",
            "vfmsubadd132pd", "vfmsubadd213pd", "vfmsubadd231pd", "vfmsubadd132ps", "vfmsubadd213ps", "vfmsubadd231ps", "vfmsubadd132sd", "vfmsubadd213sd", "vfmsubadd231sd", "vfmsubadd132ss", "vfmsubadd213ss", "vfmsubadd231ss",
            // avx2.0
            "bextr", "blsi", "blmsk", "blsr", "bzhi", "lzcnt", "movbe", "mulx", "pdep", "pext", "popcnt", "rdrand", "rdseed", "rorx", "sarx", "shlx", "shrx", "tzcnt", "andn"
		};
        private static readonly string[] lex_registersCpu =
        {
            "rax", "eax", "ax", "ah", "al", "rcx", "ecx", "cx", "ch", "cl", "rbx", "ebx", "bx", "bh", "bl",
            "rdx", "edx", "dx", "dh", "dl", "rsi", "esi", "si", "sil", "rdi", "edi", "di", "dil", "rbp", "ebp", "bp", "bpl", "rsp", "esp", "sp", "spl", "rip",
            "r8", "r9", "r10", "r11", "r12", "r13", "r14", "r15",
            "r8d", "r9d", "r10d", "r11d", "r12d", "r13d", "r14d", "r15d",
            "r8w", "r9w", "r10w", "r11w", "r12w", "r13w", "r14w", "r15w",
            "r8b", "r9b", "r10b", "r11b", "r12b", "r13b", "r14b", "r15b"
        };
        private static readonly string[] lex_registersFpu =
        {
            "st", "st0", "st1", "st2", "st3", "st4", "st5", "st6", "st7"
        };
        private static readonly string[] lex_registersExt =
        {
            "xmm0", "xmm1", "xmm2", "xmm3", "xmm4", "xmm5", "xmm6", "xmm7", "xmm8", "xmm9", "xmm10", "xmm11", "xmm12", "xmm13", "xmm14", "xmm15",
			"ymm0", "ymm1", "ymm2", "ymm3", "ymm4", "ymm5", "ymm6", "ymm7", "ymm8", "ymm9", "ymm10", "ymm11", "ymm12", "ymm13", "ymm14", "ymm15",
			"mm0", "mm1", "mm2", "mm3", "mm4", "mm5", "mm6", "mm7",
			"zmm0", "zmm1", "zmm2", "zmm3", "zmm4", "zmm5", "zmm6", "zmm7", "zmm8", "zmm9", "zmm10", "zmm11", "zmm12", "zmm13", "zmm14", "zmm15",
			"zmm16", "zmm17", "zmm18", "zmm19", "zmm20", "zmm21", "zmm22", "zmm23", "zmm24", "zmm25", "zmm26", "zmm27", "zmm28", "zmm29", "zmm30", "zmm31"
		};
		private static readonly string[] lex_directives1 =
		{
			"dd", "dq", "dw", "db", "dt"
		};
		private static readonly string[] lex_directives2 =
		{
			"proc", "struct", "macro", "equ", "union", "record"
		};
		private static readonly string[] lex_directives3 =
		{
			"word", "ptr", "dword", "qword", "byte", "@@", "@b", "@f", "offset",
			"public", "endp", "uses", "local", "include", "includelib", "if", "endif", "ifdef", "ifndef", "ifidn", "comment", "else", "elseif",
			"invoke", "near", "dup", "endm", "ymmword", "xmmword",
			"end", ".model", "proto", "extern", "flat", "stdcall", ".code", ".data", ".xmm", ".686",
			"externdef", ".data?", "option", ".const", "private", "align", "ends",
			"segment","echo", "typedef", "far"
		};
	}
	#endregion //Classifier
}
