using System.Text;
using System.Collections.Generic;

namespace PingTool
{
    public sealed class PingResult
    {
        // made out of lines
        private StringBuilder output;
        private List<StringBuilder> lines;
        private int maxLines;

        public string Output {
            get
            {
                return output.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxLines">Each line above the maxLines will delete the first one.</param>
        public PingResult(int maxLines)
        {
            this.maxLines = maxLines;
            output = new StringBuilder();
            lines = new List<StringBuilder>();
        }

        public void AddLine(StringBuilder line)
        {
            AddLineToList(line);
            UpdateOutput();
        }

        private void AddLineToList(StringBuilder line)
        {
            if (lines.Count >= maxLines)
            {
                lines.RemoveAt(0);
            }
            lines.Add(line);
        }

        public void Clear()
        {
            lines.Clear();
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            output.Remove(0, output.Length);
            for (int i = 0; i < lines.Count; i++)
            {
                output.Append(lines[i]);
            }
        }
    }
}
