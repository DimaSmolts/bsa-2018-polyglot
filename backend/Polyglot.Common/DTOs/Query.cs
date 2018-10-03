using System;
using System.Collections.Generic;
using System.Text;

namespace Polyglot.Common.DTOs
{
    public class Query
    {
		public List<int> Tags { get; set; }
		public TranslationStatus Status { get; set; }
		public string SearchQuery { get; set; }

		public Query()
		{
			Tags = new List<int>();
		}

		public enum TranslationStatus
		{
			All = 0,
			Untranslated = 1,
			InProgress = 2,
			Done = 3
		}
	}	
}
