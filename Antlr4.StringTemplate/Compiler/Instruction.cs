namespace Antlr4.StringTemplate.Compiler
{
    using System.Linq;
    using ArgumentNullException = System.ArgumentNullException;
    using Array = System.Array;
    using Enum = System.Enum;

    public sealed class Instruction
    {
        public const int MaxOperands = 2;
        public const int OperandSizeInBytes = 2;

        /// <summary>
        /// Used for assembly/disassembly; describes instruction set
        /// </summary>
        public static readonly Instruction[] instructions;

        internal readonly string name; // E.g., "load_str", "new"
        internal readonly OperandType[] type = new OperandType[MaxOperands];
        internal readonly int nopnds = 0;

        static Instruction()
        {
            Array values = Enum.GetValues(typeof(Bytecode));
            instructions = new Instruction[values.Cast<byte>().Max() + 1];

            instructions[(int)Bytecode.Invalid] = null;
            instructions[(int)Bytecode.INSTR_LOAD_STR] = new Instruction("load_str", OperandType.String);
            instructions[(int)Bytecode.INSTR_LOAD_ATTR] = new Instruction("load_attr", OperandType.String);
            instructions[(int)Bytecode.INSTR_LOAD_LOCAL] = new Instruction("load_local", OperandType.Int);
            instructions[(int)Bytecode.INSTR_LOAD_PROP] = new Instruction("load_prop", OperandType.String);
            instructions[(int)Bytecode.INSTR_LOAD_PROP_IND] = new Instruction("load_prop_ind");
            instructions[(int)Bytecode.INSTR_STORE_OPTION] = new Instruction("store_option", OperandType.Int);
            instructions[(int)Bytecode.INSTR_STORE_ARG] = new Instruction("store_arg", OperandType.String);
            instructions[(int)Bytecode.INSTR_NEW] = new Instruction("new", OperandType.String, OperandType.Int);
            instructions[(int)Bytecode.INSTR_NEW_IND] = new Instruction("new_ind", OperandType.Int);
            instructions[(int)Bytecode.INSTR_NEW_BOX_ARGS] = new Instruction("new_box_args", OperandType.String);
            instructions[(int)Bytecode.INSTR_SUPER_NEW] = new Instruction("super_new", OperandType.String, OperandType.Int);
            instructions[(int)Bytecode.INSTR_SUPER_NEW_BOX_ARGS] = new Instruction("super_new_box_args", OperandType.String);
            instructions[(int)Bytecode.INSTR_WRITE] = new Instruction("write");
            instructions[(int)Bytecode.INSTR_WRITE_OPT] = new Instruction("write_opt");
            instructions[(int)Bytecode.INSTR_MAP] = new Instruction("map");
            instructions[(int)Bytecode.INSTR_ROT_MAP] = new Instruction("rot_map", OperandType.Int);
            instructions[(int)Bytecode.INSTR_ZIP_MAP] = new Instruction("zip_map", OperandType.Int);
            instructions[(int)Bytecode.INSTR_BR] = new Instruction("br", OperandType.Address);
            instructions[(int)Bytecode.INSTR_BRF] = new Instruction("brf", OperandType.Address);
            instructions[(int)Bytecode.INSTR_OPTIONS] = new Instruction("options");
            instructions[(int)Bytecode.INSTR_ARGS] = new Instruction("args");
            instructions[(int)Bytecode.INSTR_PASSTHRU] = new Instruction("passthru", OperandType.String);
            //instructions[(int)Bytecode.INSTR_PASSTHRU_IND] = new Instruction("passthru_ind", OperandType.Int);
            instructions[(int)Bytecode.INSTR_LIST] = new Instruction("list");
            instructions[(int)Bytecode.INSTR_ADD] = new Instruction("add");
            instructions[(int)Bytecode.INSTR_TOSTR] = new Instruction("tostr");
            instructions[(int)Bytecode.INSTR_FIRST] = new Instruction("first");
            instructions[(int)Bytecode.INSTR_LAST] = new Instruction("last");
            instructions[(int)Bytecode.INSTR_REST] = new Instruction("rest");
            instructions[(int)Bytecode.INSTR_TRUNC] = new Instruction("trunc");
            instructions[(int)Bytecode.INSTR_STRIP] = new Instruction("strip");
            instructions[(int)Bytecode.INSTR_TRIM] = new Instruction("trim");
            instructions[(int)Bytecode.INSTR_LENGTH] = new Instruction("length");
            instructions[(int)Bytecode.INSTR_STRLEN] = new Instruction("strlen");
            instructions[(int)Bytecode.INSTR_REVERSE] = new Instruction("reverse");
            instructions[(int)Bytecode.INSTR_NOT] = new Instruction("not");
            instructions[(int)Bytecode.INSTR_OR] = new Instruction("or");
            instructions[(int)Bytecode.INSTR_AND] = new Instruction("and");
            instructions[(int)Bytecode.INSTR_INDENT] = new Instruction("indent", OperandType.String);
            instructions[(int)Bytecode.INSTR_DEDENT] = new Instruction("dedent");
            instructions[(int)Bytecode.INSTR_NEWLINE] = new Instruction("newline");
            instructions[(int)Bytecode.INSTR_NOOP] = new Instruction("noop");
            instructions[(int)Bytecode.INSTR_POP] = new Instruction("pop");
            instructions[(int)Bytecode.INSTR_NULL] = new Instruction("null");
            instructions[(int)Bytecode.INSTR_TRUE] = new Instruction("true");
            instructions[(int)Bytecode.INSTR_FALSE] = new Instruction("false");
            instructions[(int)Bytecode.INSTR_WRITE_STR] = new Instruction("write_str", OperandType.String);
            instructions[(int)Bytecode.INSTR_WRITE_LOCAL] = new Instruction("write_local", OperandType.Int);
        }

        public Instruction(string name)
            : this(name, OperandType.None, OperandType.None)
        {
            nopnds = 0;
        }

        public Instruction(string name, OperandType a)
            : this(name, a, OperandType.None)
        {
            nopnds = 1;
        }

        public Instruction(string name, OperandType a, OperandType b)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
            type[0] = a;
            type[1] = b;
            nopnds = MaxOperands;
        }
    }
}
