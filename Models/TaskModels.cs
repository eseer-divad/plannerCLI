using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plannerCLI.Models
{
    public abstract class TaskModel
    {
        public int Id { get; set; }
    }

    public class StandardTaskModel : TaskModel
    {
        public string TaskName { get; set; }
        public DateTime Due {  get; set; }
        public int Priority { get; set; }
        public string Note { get; set; }
        public DateTime Added { get; set; }
    }
}
