using System.Collections.Generic;

namespace Polyglot.Common.DTOs
{
    public class GlossaryDTO
    {
        public int Id { get; set; }

        public virtual ICollection<GlossaryStringDTO> GlossaryStrings { get; set; }

        public string OriginLanguage { get; set; }
        
        public List<GlossaryDTO> ProjectGlossaries { get; set; }

        public GlossaryDTO()
        {
            GlossaryStrings = new List<GlossaryStringDTO>();
            ProjectGlossaries = new List<GlossaryDTO>();
        }
    }
}
