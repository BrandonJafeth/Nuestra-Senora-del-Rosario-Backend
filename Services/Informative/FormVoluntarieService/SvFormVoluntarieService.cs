using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.Informative.GenericRepository;
using Services.MyDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.FormVoluntarieServices
{
    public class SvFormVoluntarieService : SvGenericRepository<FormVoluntarie>, ISvFormVoluntarieService
    {
        private readonly MyContext _context;

        public SvFormVoluntarieService(MyContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormVoluntarie>> GetFormVoluntariesWithTypesAsync()
        {
            return await _context.FormVoluntaries
                .Include(f => f.VoluntarieType)
                .ToListAsync();
        }
    }
}