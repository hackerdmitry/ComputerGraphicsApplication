using System.Drawing;

namespace ComputerGraphicsApplication.Models.Filters
{
    public abstract class AbstractFilter
    {
        public abstract Bitmap Apply(Bitmap bitmap);
    }
}