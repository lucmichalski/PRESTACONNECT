
namespace PRESTACONNECT.Core.Parametres
{
    public enum RegexMail
    {
        lvl00_ld = 0, // lowercase and digit
        lvl04_lUd = 4, // lowercase, Uppercase and digit
        lvl08_lUdS = 8, // lowercase, Uppercase, digit and Special characters
        lvl12_lUAdS = 12, // lowercase, Uppercase, Accented characters, digit and following Special characters !#$%&'*+/=?^_`{|}~-
        lvl16_Q = 16, // All characters IF QUOTED "example"@domain.region
    }
}