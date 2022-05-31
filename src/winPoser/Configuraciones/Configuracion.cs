using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using winPoser.Configuraciones;

namespace winPoser
{
    public class Configuracion
    {
        public List<Packs> PACKS = new List<Packs>();
        public List<PHP> PHP_INSTALLED = new List<PHP>();

        public Packs getPack(string nombre) {
            foreach(Packs p in PACKS)
            {
                if (p.nombre == nombre)
                {
                    return p;
                }
            }
            return null;
        }
        public bool removePack(string packname) {
            foreach (Packs p in PACKS) {
                if (p.nombre == packname)
                {
                    PACKS.Remove(p);
                    return true;
                }
            }
            return false;
        }
        public  PHP getPHP(string version_number,string arc) {
            foreach(PHP p in PHP_INSTALLED)
            {
                if (p.version==version_number && p.arquitectura==arc)
                {
                    return p;
                }
            }
            return null;
        }
    }
}
