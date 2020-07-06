using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorsAdModel;
using RestApiClientAdsServices.Api;

namespace MotorsWeb.Controllers
{
    public class AdsController : Controller
    {
        private readonly IAdsApi _adsApi;

        public AdsController(IAdsApi adsApi)
        {
            _adsApi = adsApi;
        }

        // GET: Ads
        public async Task<IActionResult> Index()
        {
            var ret = await _adsApi.ApiAdsGetAsync();
            return View(TransformDataList(ret));
        }

        // GET: Ads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _adsApi.ApiAdsIdGetAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            return View(TransformData2Model(ad));
        }

        // GET: Ads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,marca,modelo,versao,ano,quilometragem,observacao")] MotorsAdModel.Ad ad)
        {
            if (ModelState.IsValid)
            {
                await _adsApi.ApiAdsPostAsync(TransformData2Rest(ad));

                return RedirectToAction(nameof(Index));
            }
            return View(ad);
        }

        // GET: Ads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _adsApi.ApiAdsIdGetAsync(id);

            if (ad == null)
            {
                return NotFound();
            }
            return View(TransformData2Model(ad));
        }

        // POST: Ads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,marca,modelo,versao,ano,quilometragem,observacao")] MotorsAdModel.Ad ad)
        {
            if (id != ad.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _adsApi.ApiAdsIdPutAsync(id, TransformData2Rest(ad));

                return RedirectToAction(nameof(Index));
            }
            return View(ad);
        }

        // GET: Ads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ad = await _adsApi.ApiAdsIdGetAsync(id);

            if (ad == null)
            {
                return NotFound();
            }

            return View(TransformData2Model(ad));
        }

        // POST: Ads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _adsApi.ApiAdsIdDeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(int id)
        {
            var ad = _adsApi.ApiAdsIdGet(id);

            return ad == null ? false : true;
        }

        /// <summary>
        /// TransformData2Model
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Ad TransformData2Model(RestApiClientAdsServices.Model.Ad item)
        {
            Ad ret = new Ad
            {
                ID = item.Id.Value,
                ano = item.Ano.Value,
                marca = item.Marca,
                modelo = item.Modelo,
                observacao = item.Observacao,
                quilometragem = item.Quilometragem.Value,
                versao = item.Versao
            };

            return ret;
        }

        /// <summary>
        /// TransformData2Rest
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private RestApiClientAdsServices.Model.Ad TransformData2Rest(Ad item)
        {
            RestApiClientAdsServices.Model.Ad data = new RestApiClientAdsServices.Model.Ad(item.ID, item.marca, item.modelo, item.versao, item.ano, item.quilometragem, item.observacao);
            return data;
        }

        /// <summary>
        /// TransformDataList
        /// </summary>
        /// <param name="lista"></param>
        /// <returns></returns>
        private List<Ad> TransformDataList(List<RestApiClientAdsServices.Model.Ad> lista)
        {
            List<Ad> list = new List<Ad>();
            foreach (var item in lista)
            {
                list.Add(new Ad
                {
                    ID = item.Id.Value,
                    ano = item.Ano.Value,
                    marca = item.Marca,
                    modelo = item.Modelo,
                    observacao = item.Observacao,
                    quilometragem = item.Quilometragem.Value,
                    versao = item.Versao
                });
            }
            return list;
        }
    }
}
