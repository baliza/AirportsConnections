using System.Collections.Generic;
using System.Text.RegularExpressions;

//https://bitbucket.org/boriscosic/backend-flitetrakr/src/0ee9ae830663fd24fd162986da57b7c931414817/src/main/java/com/boriscosic/flitetrakr/query/Question.java?at=master&fileviewer=file-view-default

namespace AirportsConnections.Library.Queries
{
    public class Question
    {
        public Question(string str)
        {
            var question = str.Replace("(?i)\\s+to\\s+|\\s+and\\s+", "-");
            SetType(question);
            SetNumber(question);
            SetValue(question);
            SetRoute(question);
            
        }

        public int Number { get; private set; }
        public string Route { get; private set; }
        public Queries.Type Type { get; private set; }
        public int Value { get; private set; }

        /// <summary>
        /// Extract query number for displaying with answer
        /// </summary>
        /// <param name="question"></param>
        private void SetNumber(string question)
        {
            /*
               String[] parts = question.split(" ");
        String digits = parts[0].replaceAll("[^\\d]", "");
        number = digits.length() > 0 && parts.length > 0 ? Integer.parseInt(digits) : Integer.MAX_VALUE;

             */
            const string pattern = "\\d+";
            var regex = new Regex(pattern);
            var matcher = regex.Match(question);
            var num = int.MaxValue;
            if (matcher.Success)
                int.TryParse(matcher.Groups[0].Value, out num);
            Number = num;
        }

        /// <summary>
        /// Extract and normalize route to lookup
        /// </summary>
        /// <param name="question"></param>
        private void SetRoute(string question)
        {
            Route = string.Empty;

            //Pattern pattern = Pattern.compile("([A-Z]{3})(-[A-Z]{3}){1,9}(\\s|\\?$|$)");
            //Matcher matcher = pattern.matcher(question);
            var q = replace(question);

            const string pattern = "([A-Z]{3})(-[A-Z]{3}){1,9}(\\s|\\?$|$)";
            var regex = new Regex(pattern);
            var matcher = regex.Match(q);

            if (matcher.Success)
                Route = matcher.Groups[0].Value.Replace("?", "").Trim();
        }

        //https://msdn.microsoft.com/en-us/library/ewy2t5e0(v=vs.110).aspx
        private string replace(string input)
        {
            return input.ToUpperInvariant().Replace(" AND ", "-").Replace(" TO ", "-");
            var pattern = ".*to*.";
            string replacement = "-";
            Regex rgx = new Regex(pattern);

            string result = rgx.Replace(input, replacement);
            var s = Regex.Replace(input, ".*to*.", m => "-");
            return result;
        }

        /// <summary>
        /// Determine type of query in order to run the proper path finding / traversal on graph
        /// </summary>
        /// <param name="question"></param>
        private void SetType(string question)
        {
            Type = Queries.Type.Unknown;
            var q = replace(question);

            var patterns = new Dictionary<Type, string>
            {
                {Queries.Type.Price, "(?i:.*is*.*price.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))"},
                {Queries.Type.Cheapest, "(?i:.*is*.*cheapest.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))"},
                {Queries.Type.MaximumConnections, "(?i:.*with*.*maximum.*\\d.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))"},
                {Queries.Type.MinimumConnections, "(?i:.*with*.*minimum.*\\d.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))"},
                {Queries.Type.ExactConnections, "(?i:.*with*.*exact.*\\d.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))"},
                {Queries.Type.BelowPrice, "(?i:.*([A-Z]{3}-[A-Z]{3})(\\s).*below.*\\d*.)"}
            };

            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern.Value);
                var matcher = regex.Match(q);
                if (matcher.Success)
                {
                    Type = pattern.Key;
                    break;
                }
            }

            //Map<Type, String> patterns = new HashMap<Type, String>();
            //patterns.put(Type.PRICE, "(?i:.*is*.*price.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))");
            //patterns.put(Type.CHEAPEST, "(?i:.*is*.*cheapest.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))");
            //patterns.put(Type.MAXIMUM_CONNECTIONS, "(?i:.*with*.*maximum.*\\d.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))");
            //patterns.put(Type.MINIMUM_CONNECTIONS, "(?i:.*with*.*minimum.*\\d.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))");
            //patterns.put(Type.EXACT_CONNECTIONS, "(?i:.*with*.*exact.*\\d.*([A-Z]{3}-[A-Z]{3})(\\s|\\?$|$))");
            //patterns.put(Type.BELOW_PRICE, "(?i:.*([A-Z]{3}-[A-Z]{3})(\\s).*below.*\\d*.)");

            //for (Type key : patterns.keySet())
            //{
            //    Pattern pattern = Pattern.compile(patterns.get(key));
            //    Matcher matcher = pattern.matcher(question);
            //    if (matcher.find())
            //        type = key;
            //}

            //if (type == null) type = Type.UNKNOWN;
        }

        /// <summary>
        /// Extract any numerical values out of the query that may be needed
        /// </summary>
        /// <param name="question"></param>
        private void SetValue(string question)
        {
            //Pattern pattern = Pattern.compile("\\d+");
            //Matcher matcher = pattern.matcher(question.replaceAll("#\\d+: ", ""));

            const string pattern = "\\d+";
            var regex = new Regex(pattern);
            var matcher = regex.Match(question);
            var num = 0;
            if (matcher.Success)
            {
                var v = matcher.Groups[0].Value;
                var idx = matcher.Groups[0].Index + v.Length;
                matcher = regex.Match(question.Substring(idx));
                int.TryParse(matcher.Groups[0].Value, out num);
            }
            Value = num;
        }
    }
}