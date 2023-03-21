using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FriendsGamesTools.CodeGeneration
{
    public abstract class GenerationRequirement
    {
        public GenerationRequirement parent { get; private set; }
        protected List<GenerationRequirement> children = new List<GenerationRequirement>();
        public bool complete => completeSelf && children.All(c => c.complete);
        protected abstract bool completeSelf { get; }
        public T AddAndGet<T>(T item) where T : GenerationRequirement
        {
            item.parent = this;
            children.Add(item);
            return item;
        }
        public FileRequirement parentFile => (this is FileRequirement f) ? f : parent?.parentFile;
        public StringBuilder sb => parentFile.fileStringBuilder;
        public string indent1 => "    ";
        public string indent2 => "        ";
        public string indent3 => "            ";
        public virtual void Generate() => children.ForEach(c => c.Generate());
    }
}