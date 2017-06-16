using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FeedService.DbModels
{
    public enum FeedType
    {
        RSS,
        RDF,
        Atom        
    }
}
