/*
 * https://bitbucket.org/boriscosic/backend-flitetrakr/src/0ee9ae830663fd24fd162986da57b7c931414817/src/main/java/com/boriscosic/flitetrakr/answer/Answer.java?at=master&fileviewer=file-view-default
 */

namespace AirportsConnections.Library.Answers
{
    // Class for formatting answers to questions.
    public class Answer
    {
        /// <summary>
        /// Generic format for each answer.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public static string Format(int number, string answer)
        {
            if (string.IsNullOrEmpty(answer))
                return $"{number}: No such connection found!";
            if (number == int.MinValue)
                return number.ToString();
            return $"#{number}: {answer}";
        }

        /// <summary>
        /// Return an answer for an unknown query.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public static string UnknownQuestion(string question)
        {
            return "I don't understand query: " + question;
        }
    }
}