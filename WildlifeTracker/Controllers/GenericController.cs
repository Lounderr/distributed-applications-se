using Microsoft.AspNetCore.Mvc;

using WildlifeTracker.Data;

namespace WildlifeTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<T> : ControllerBase where T : class
    {
        private readonly List<T> _items = new();
        private readonly ApplicationDbContext context;

        public GenericController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return this.Ok(this._items);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id < 0 || id >= this._items.Count)
                return this.NotFound();

            return this.Ok(this._items[id]);
        }

        [HttpPost]
        public IActionResult Create([FromBody] T item)
        {
            if (item == null)
                return this.BadRequest();

            this._items.Add(item);
            return this.CreatedAtAction(nameof(GetById), new { id = this._items.Count - 1 }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] T item)
        {
            if (id < 0 || id >= this._items.Count || item == null)
                return this.BadRequest();

            this._items[id] = item;
            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0 || id >= this._items.Count)
                return this.NotFound();

            this._items.RemoveAt(id);
            return this.NoContent();
        }
    }
}
