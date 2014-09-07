/* Generated By:CSharpCC: Do not edit this line. WikiParserConstants.cs */
namespace Conclave.Parsing.Wiki{
public  class WikiParserConstants {

  public const int EOF = 0;
  public const int URL = 1;
  public const int TOPIC_LINK = 2;
  public const int LINK = 3;
  public const int HEADING = 4;
  public const int OPEN_HEADING = 5;
  public const int CLOSE_HEADING = 6;
  public const int BLOCK_BOX = 7;
  public const int BLOCK_QUOTE = 8;
  public const int BLOCK_FORM = 9;
  public const int OPEN_BLOCK_BOX = 10;
  public const int OPEN_BLOCK_QUOTE = 11;
  public const int OPEN_BLOCK_FORM = 12;
  public const int CLOSE_BLOCK_BOX = 13;
  public const int CLOSE_BLOCK_QUOTE = 14;
  public const int CLOSE_BLOCK_FORM = 15;
  public const int BLOCK_CODE = 16;
  public const int OPEN_BLOCK_CODE = 17;
  public const int CLOSE_BLOCK_CODE = 18;
  public const int CODE_TEXT = 19;
  public const int BLOCK_TITLE = 20;
  public const int BLOCK_TABLE = 21;
  public const int OPEN_BLOCK_TABLE = 22;
  public const int CLOSE_BLOCK_TABLE = 23;
  public const int UNKNOWN = 24;
  public const int NONASCII = 25;
  public const int OPEN_ANCHOR = 26;
  public const int CLOSE_ANCHOR = 27;
  public const int CONTROLS_AND_BASIC_LATIN = 28;
  public const int CONTROLS_AND_LATIN1_SUPPLEMENT = 29;
  public const int LATIN_EXTENDED_A = 30;
  public const int LATIN_EXTENDED_B = 31;
  public const int LATIN_EXTENDED_ADDITIONAL = 32;
  public const int GENERAL_PUNCTUATION = 33;
  public const int SUPER_AND_SUB_SCRIPT = 34;
  public const int CURRENCY_SYMBOL = 35;
  public const int CYRILLIC = 36;
  public const int CYRILLIC_SUPPLEMENT = 37;
  public const int UNICODE_RANGES = 38;
  public const int WIKI_NAME = 39;
  public const int NUMBER = 40;
  public const int ALPHA_NUMERIC = 41;
  public const int DIGIT = 42;
  public const int UPPER = 43;
  public const int LOWER = 44;
  public const int ALPHA = 45;
  public const int SOLID_TEXT = 46;
  public const int TEXT = 47;
  public const int SAFE_TEXT = 48;
  public const int SPACE = 49;
  public const int TAB = 50;
  public const int WS = 51;
  public const int NL = 52;
  public const int NON_MARKUP_SYMBOL = 53;
  public const int MARKUP_SYMBOL = 54;
  public const int ESCAPED_SYMBOL = 55;
  public const int SPECIAL_SYMBOL = 56;
  public const int SYMBOL = 57;
  public const int SAFE_SYMBOL = 58;
  public const int FURTHER = 59;
  public const int HYPHEN = 60;
  public const int EO = 61;
  public const int FOREIGN = 62;
  public const int PLUS = 63;
  public const int MINUS = 64;
  public const int MULTIPLY = 65;
  public const int EQUALS = 66;
  public const int TILDE = 67;
  public const int HASH = 68;
  public const int HAT = 69;
  public const int ACUTE = 70;
  public const int BAR = 71;
  public const int OPEN_INDEX = 72;
  public const int CLOSE_INDEX = 73;
  public const int LT = 74;
  public const int GT = 75;
  public const int DIVIDE = 76;
  public const int PERCENT = 77;
  public const int APOS = 78;
  public const int QUOTE = 79;
  public const int POUND = 80;
  public const int DOLAR = 81;
  public const int AMP = 82;
  public const int OPEN_PAREN = 83;
  public const int CLOSE_PAREN = 84;
  public const int OPEN_BRACE = 85;
  public const int CLOSE_BRACE = 86;
  public const int COLON = 87;
  public const int SEMICOLON = 88;
  public const int AT = 89;
  public const int COMMA = 90;
  public const int POINT = 91;
  public const int QUESTION = 92;
  public const int EXCLAIM = 93;
  public const int BACKSLASH = 94;
  public const int UNDERSCORE = 95;
  public const int ESC_PLUS = 96;
  public const int ESC_MINUS = 97;
  public const int ESC_MULTIPLY = 98;
  public const int ESC_EQUALS = 99;
  public const int ESC_TILDE = 100;
  public const int ESC_HASH = 101;
  public const int ESC_HAT = 102;
  public const int ESC_ACUTE = 103;
  public const int ESC_BAR = 104;
  public const int ESC_OPEN_INDEX = 105;
  public const int ESC_CLOSE_INDEX = 106;
  public const int L_SINGLE_QUOTE = 107;
  public const int R_SINGLE_QUOTE = 108;
  public const int SINGLE_LOW_QUOTE = 109;
  public const int L_DOUBLE_QUOTE = 110;
  public const int R_DOUBLE_QUOTE = 111;
  public const int NDASH = 112;
  public const int MDASH = 113;
  public const int BULLET = 114;
  public const int EURO = 115;

  public const int IN_TABLE = 0;
  public const int IN_CODE = 1;
  public const int IN_BLOCK = 2;
  public const int IN_HEADING = 3;
  public const int DEFAULT = 4;

  public readonly string[] tokenImage = {
    "<EOF>",
    "<URL>",
    "<TOPIC_LINK>",
    "<LINK>",
    "<HEADING>",
    "<OPEN_HEADING>",
    "<CLOSE_HEADING>",
    "<BLOCK_BOX>",
    "<BLOCK_QUOTE>",
    "<BLOCK_FORM>",
    "<OPEN_BLOCK_BOX>",
    "<OPEN_BLOCK_QUOTE>",
    "<OPEN_BLOCK_FORM>",
    "<CLOSE_BLOCK_BOX>",
    "<CLOSE_BLOCK_QUOTE>",
    "<CLOSE_BLOCK_FORM>",
    "<BLOCK_CODE>",
    "<OPEN_BLOCK_CODE>",
    "<CLOSE_BLOCK_CODE>",
    "<CODE_TEXT>",
    "<BLOCK_TITLE>",
    "<BLOCK_TABLE>",
    "<OPEN_BLOCK_TABLE>",
    "<CLOSE_BLOCK_TABLE>",
    "<UNKNOWN>",
    "<NONASCII>",
    "\"<<\"",
    "\">>\"",
    "<CONTROLS_AND_BASIC_LATIN>",
    "<CONTROLS_AND_LATIN1_SUPPLEMENT>",
    "<LATIN_EXTENDED_A>",
    "<LATIN_EXTENDED_B>",
    "<LATIN_EXTENDED_ADDITIONAL>",
    "<GENERAL_PUNCTUATION>",
    "<SUPER_AND_SUB_SCRIPT>",
    "<CURRENCY_SYMBOL>",
    "<CYRILLIC>",
    "<CYRILLIC_SUPPLEMENT>",
    "<UNICODE_RANGES>",
    "<WIKI_NAME>",
    "<NUMBER>",
    "<ALPHA_NUMERIC>",
    "<DIGIT>",
    "<UPPER>",
    "<LOWER>",
    "<ALPHA>",
    "<SOLID_TEXT>",
    "<TEXT>",
    "<SAFE_TEXT>",
    "\" \"",
    "\"\\t\"",
    "<WS>",
    "<NL>",
    "<NON_MARKUP_SYMBOL>",
    "<MARKUP_SYMBOL>",
    "<ESCAPED_SYMBOL>",
    "<SPECIAL_SYMBOL>",
    "<SYMBOL>",
    "<SAFE_SYMBOL>",
    "\":-\"",
    "\" - \"",
    "\"\\u0153\"",
    "<FOREIGN>",
    "\"+\"",
    "\"-\"",
    "\"*\"",
    "\"=\"",
    "\"~\"",
    "\"#\"",
    "\"^\"",
    "\"`\"",
    "\"|\"",
    "\"[\"",
    "\"]\"",
    "\"<\"",
    "\">\"",
    "\"/\"",
    "\"%\"",
    "\"\\\'\"",
    "\"\\\"\"",
    "\"\\u00c2\\u00a3\"",
    "\"$\"",
    "\"&\"",
    "\"(\"",
    "\")\"",
    "\"{\"",
    "\"}\"",
    "\":\"",
    "\";\"",
    "\"@\"",
    "\",\"",
    "\".\"",
    "\"?\"",
    "\"!\"",
    "\"\\\\\"",
    "\"_\"",
    "\"\\\\+\"",
    "\"\\\\-\"",
    "\"\\\\*\"",
    "\"\\\\=\"",
    "\"\\\\~\"",
    "\"\\\\#\"",
    "\"\\\\^\"",
    "\"\\\\`\"",
    "\"\\\\|\"",
    "\"\\\\[\"",
    "\"\\\\]\"",
    "\"\\u2018\"",
    "\"\\u2019\"",
    "\"\\u201a\"",
    "\"\\u201c\"",
    "\"\\u201d\"",
    "\"\\u2013\"",
    "\"\\u2014\"",
    "\"\\u2022\"",
    "\"\\u20ac\"",
  };

}
}