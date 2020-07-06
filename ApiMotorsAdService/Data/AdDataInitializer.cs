using MotorsAdModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMotorsAdService.Data
{
    public class AdDataInitializer
    {
        public static void SeedData(ServiceDbContext dbContext)
        {
            SeedCategories(dbContext);
        }

        private static void SeedCategories(ServiceDbContext dbContext)
        {
            if (dbContext.tb_AnuncioWebmotors.FirstOrDefault() == null)
            {
                dbContext.tb_AnuncioWebmotors.Add(new Ad()
                {
                    marca = "Honda",
                    modelo = "City",
                    ano = 2018,
                    versao = "2.0 EXL 4X4 16V GASOLINA 4P AUTOMÁTICO",
                    quilometragem = 0,
                    observacao = "http://desafioonline.webmotors.com.br/content/img/01.jpg"
                });

                dbContext.tb_AnuncioWebmotors.Add(new Ad()
                {
                    marca = "Mitsubishi",
                    modelo = "Lancer",
                    ano = 2012,
                    versao = "2.0 EVO 4P AUTOMÁTICO",
                    quilometragem = 47500,
                    observacao = "http://desafioonline.webmotors.com.br/content/img/02.jpg"
                });

                dbContext.tb_AnuncioWebmotors.Add(new Ad()
                {
                    marca = "Chevrolet",
                    modelo = "Agile",
                    ano = 2014,
                    versao = "1.4 MPFI EFFECT 8V FLEX 4P AUTOMATIZADO",
                    quilometragem = 12000,
                    observacao = "http://desafioonline.webmotors.com.br/content/img/06.jpg"
                });

                dbContext.SaveChanges();
            }
        }
    }
}

