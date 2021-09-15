// GE Aviation Systems LLC licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GEAviation.Metrology
{
    // The classes in this file represent scaling prefixes built-in to this
    // library.

    public sealed class Yotta : IPrefix
    {
        public string Name => "yotta";
        public string Abbreviation => "Y";

        public decimal Scale => 1_000_000_000_000_000_000_000_000m;
    }

    public sealed class Zetta : IPrefix
    {
        public string Name => "Zetta";
        public string Abbreviation => "Z";

        public decimal Scale => 1_000_000_000_000_000_000_000m;
    }

    public sealed class Exa : IPrefix
    {
        public string Name => "exa";
        public string Abbreviation => "E";

        public decimal Scale => 1_000_000_000_000_000_000m;
    }

    public sealed class Peta : IPrefix
    {
        public string Name => "peta";
        public string Abbreviation => "P";

        public decimal Scale => 1_000_000_000_000_000m;
    }

    public sealed class Tera : IPrefix
    {
        public string Name => "tera";
        public string Abbreviation => "T";

        public decimal Scale => 1_000_000_000_000m;
    }

    public sealed class Giga : IPrefix
    {
        public string Name => "giga";
        public string Abbreviation => "G";

        public decimal Scale => 1_000_000_000m;
    }

    public sealed class Mega : IPrefix
    {
        public string Name => "mega";
        public string Abbreviation => "M";

        public decimal Scale => 1_000_000m;
    }

    public sealed class Kilo : IPrefix
    {
        public string Name => "kilo";
        public string Abbreviation => "k";

        public decimal Scale => 1_000m;
    }

    public sealed class Hecto : IPrefix
    {
        public string Name => "hecto";
        public string Abbreviation => "h";

        public decimal Scale => 100m;
    }

    public sealed class Deca : IPrefix
    {
        public string Name => "deca";
        public string Abbreviation => "da";

        public decimal Scale => 10m;
    }

    public sealed class NoPrefix : IPrefix
    {
        public string Name => "";
        public string Abbreviation => "";

        public decimal Scale => 1m;
    }

    public sealed class Deci : IPrefix
    {
        public string Name => "deci";
        public string Abbreviation => "d";

        public decimal Scale => 0.1m;
    }

    public sealed class Centi : IPrefix
    {
        public string Name => "centi";
        public string Abbreviation => "c";

        public decimal Scale => 0.01m;
    }

    public sealed class Milli : IPrefix
    {
        public string Name => "milli";
        public string Abbreviation => "m";

        public decimal Scale => 0.001m;
    }

    public sealed class Micro : IPrefix
    {
        public string Name => "micro";
        public string Abbreviation => "µ";

        public decimal Scale => 0.000_001m;
    }

    public sealed class Nano : IPrefix
    {
        public string Name => "nano";
        public string Abbreviation => "n";

        public decimal Scale => 0.000_000_001m;
    }

    public sealed class Pico : IPrefix
    {
        public string Name => "pico";
        public string Abbreviation => "p";

        public decimal Scale => 0.000_000_000_001m;
    }

    public sealed class Femto : IPrefix
    {
        public string Name => "femto";
        public string Abbreviation => "f";

        public decimal Scale => 0.000_000_000_000_001m;
    }

    public sealed class Atto : IPrefix
    {
        public string Name => "atto";
        public string Abbreviation => "a";

        public decimal Scale => 0.000_000_000_000_000_001m;
    }

    public sealed class Zepto : IPrefix
    {
        public string Name => "zepto";
        public string Abbreviation => "z";

        public decimal Scale => 0.000_000_000_000_000_000_001m;
    }

    public sealed class Yocto : IPrefix
    {
        public string Name => "yocto";
        public string Abbreviation => "y";

        public decimal Scale => 0.000_000_000_000_000_000_000_001m;
    }

    // For data storage...

    public sealed class Yobi : IPrefix
    {
        public string Name => "yobi";
        public string Abbreviation => "Yi";

        public decimal Scale => 1_208_925_819_614_629_174_706_176m;
    }

    public sealed class Zebi : IPrefix
    {
        public string Name => "zebi";
        public string Abbreviation => "Zi";

        public decimal Scale => 1_180_591_620_717_411_303_424m;
    }

    public sealed class Exbi : IPrefix
    {
        public string Name => "exbi";
        public string Abbreviation => "Ei";

        public decimal Scale => 1_152_921_504_606_846_976m;
    }

    public sealed class Pebi : IPrefix
    {
        public string Name => "pebi";
        public string Abbreviation => "Pi";

        public decimal Scale => 1_125_899_906_842_624m;
    }

    public sealed class Tebi : IPrefix
    {
        public string Name => "tebi";
        public string Abbreviation => "Ti";

        public decimal Scale => 1_099_511_627_776m;
    }

    public sealed class Gibi : IPrefix
    {
        public string Name => "gibi";
        public string Abbreviation => "Gi";

        public decimal Scale => 1_073_741_824m;
    }

    public sealed class Mebi : IPrefix
    {
        public string Name => "mebi";
        public string Abbreviation => "Mi";

        public decimal Scale => 1_048_576m;
    }

    public sealed class Kibi : IPrefix
    {
        public string Name => "kibi";
        public string Abbreviation => "Ki";

        public decimal Scale => 1_024m;
    }
}
