using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PassportProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            // Puzzle 1
            //var items = ReadInput("TestData.txt");
            //var items = ReadInput("Input1.txt");

            // Puzzle 2
            //var items = ReadInput("Invalid.txt");
            //var items = ReadInput("Valid.txt");
            var items = ReadInput("Input1.txt");
            var validPassports = items.Count(x => x.IsValidPassport == true);

            Console.WriteLine($"Valid count: {validPassports}");
        }


        static IList<CredentialEntry> ReadInput(string filename)
        {
            StreamReader reader = null;
            string[] input;

            try
            {
                using (reader = new StreamReader(filename))
                {
                    input = reader.ReadToEnd().Split(Environment.NewLine, StringSplitOptions.None);
                }
            }
            finally
            {
                reader?.Close();
            }

            IList<CredentialEntry> result = new List<CredentialEntry>();
            CredentialEntry current = new CredentialEntry();

            for (int x = 0; x < input.Length; x++)
            {
                if (input[x].Length == 0 && x < input.Length - 1)
                {
                    result.Add(current);
                    current = new CredentialEntry();
                    continue;
                }

                var pairs = input[x].Split(" ");

                foreach (var entry in pairs)
                {
                    var kvp = entry.Split(":");
                    current.Fields.Add(kvp[0], kvp[1]);
                }
            }

            if(current.Fields.Count > 0)
                result.Add(current);

            return result;
        }
    }


    class CredentialEntry
    {
        public Dictionary<string, string> Fields = new Dictionary<string, string>();

        private HashSet<string> ValidEyeColors = new HashSet<string>()
        {
            "amb", "blu", "brn", "gry", "grn", "hzl", "oth"
        };

        private HashSet<char> HexChars = new HashSet<char>()
        {
            'a', 'b', 'c', 'd', 'e', 'f'
        };


        /// <summary>
        /// Do we have a valid passport?
        /// </summary>
        public bool IsValidPassport
        {
            get
            {
                var correct = Fields.ContainsKey("byr") && Fields.ContainsKey("iyr") && Fields.ContainsKey("eyr") && Fields.ContainsKey("hgt") &&
                              Fields.ContainsKey("hcl") && Fields.ContainsKey("ecl") && Fields.ContainsKey("pid");

                // Puzzle 1
                // return correct;

                // Puzzle 2
                if (!correct)
                    return false;

                // Validate the fields (these could be parsed into members, but I didn't
                // bother for the sake of this challenge)

                // byr (Birth Year) - four digits; at least 1920 and at most 2002.
                if (!int.TryParse(Fields["byr"], out int byr)) return false;
                if (byr < 1920 || byr > 2002) return false;

                // iyr (Issue Year) - four digits; at least 2010 and at most 2020.
                if (!int.TryParse(Fields["iyr"], out int iyr)) return false;
                if (iyr < 2010 || iyr > 2020) return false;

                // eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
                if (!int.TryParse(Fields["eyr"], out int eyr)) return false;
                if (eyr < 2020 || eyr > 2030) return false;

                // hgt (Height) - a number followed by either cm or in:
                //   If cm, the number must be at least 150 and at most 193.
                //   If in, the number must be at least 59 and at most 76.
                if (!ValidateHeight(Fields["hgt"]))
                    return false;

                // hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
                if (!ValidateHex(Fields["hcl"]))
                    return false;

                // ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
                if (!ValidEyeColors.Contains(Fields["ecl"].ToLowerInvariant()))
                    return false;

                // pid (Passport ID) - a nine-digit number, including leading zeroes.
                if (!IsValidPassportNumber(Fields["pid"]))
                    return false;

                // cid (Country ID) - ignored, missing or not.

                return true;
            }
        }

        private bool IsValidPassportNumber(string input)
        {
            return input.Count(x => char.IsNumber(x)) == 9;
        }

        private bool ValidateHex(string input)
        {
            if (input.Length != 7 || input[0] != '#')
                return false;

            return input.Count(x => char.IsNumber(x) || HexChars.Contains(char.ToLower(x))) == 6;
        }

        private bool ValidateHeight(string hgt)
        {
            if(hgt.Length == 2)
                return false;

            var inches = hgt.EndsWith("in", StringComparison.OrdinalIgnoreCase);
            var cm = hgt.EndsWith("cm", StringComparison.OrdinalIgnoreCase);

            if (!inches && !cm)
                return false;

            if (!int.TryParse(hgt.Substring(0, hgt.Length - 2), out var number))
                return false;

            // Inches
            if (hgt.EndsWith("in", StringComparison.OrdinalIgnoreCase))
                return number >= 59 && number <= 76;
            
            // Centimeters
            return number >= 150 && number <= 193;
        }


        /// <summary>
        /// Do we have north pole credentials?
        /// </summary>
        public bool IsNorthPole
        {
            get
            {
                return IsValidPassport && !Fields.ContainsKey("cid");
            }
        }
    }
}
