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
            SetValue(question);
            SetRoute(question);
            SetNumber(question);
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
            var parts = question.Split(' ');
            var digits = parts[0].Replace("[^\\d]", "");
            var isNumber = digits.Length > 0 && parts.Length > 0;
            Number = isNumber ? int.Parse(digits) : int.MaxValue;
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

            const string pattern = "([A-Z]{3})(-[A-Z]{3}){1,9}(\\s|\\?$|$)";
            var regex = new Regex(pattern);
            var matcher = regex.Match(question);

            if (matcher.Success)
                Route = matcher.Groups[0].Value.Replace("?", "").Trim();
        }

        /// <summary>
        /// Determine type of query in order to run the proper path finding / traversal on graph
        /// </summary>
        /// <param name="question"></param>
        private void SetType(string question)
        {
            Type = Queries.Type.Unknown;

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
                var matcher = regex.Match(question);
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
            var input = question.Replace("#\\d+: ", string.Empty);
            var matcher = regex.Match(input);
            Value = matcher.Success ? int.Parse(matcher.Groups[0].Value) : 0;
        }
    }
}