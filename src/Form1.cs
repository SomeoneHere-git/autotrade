using System.Windows.Forms;

namespace AutoSalonApp
{
    public partial class Form1 : Form
    {
        private AutoSalon salon = new AutoSalon();

        public Form1()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            comboBrand.DataSource = Enum.GetValues(typeof(Brand));
        }

        // ДОДАТИ АВТО
        private void btnAddCar_Click(object sender, EventArgs e)
        {
            var parts = new Dictionary<Part, Condition>
            {
                { Part.Engine, Condition.Good },
                { Part.Body, Condition.Good }
            };

            var car = new UsedCar
            {
                Brand = (Brand)comboBrand.SelectedItem,
                Year = int.Parse(txtYear.Text),
                Price = decimal.Parse(txtPrice.Text),
                Mileage = 100000,
                Parts = parts
            };

            salon.Cars.Add(car);
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = salon.Cars;
        }

        // ЗБЕРЕГТИ
        private void btnSave_Click(object sender, EventArgs e)
        {
            salon.SaveToJson("cars.json");
            MessageBox.Show("Збережено!");
        }

        // ЗАВАНТАЖИТИ
        private void btnLoad_Click(object sender, EventArgs e)
        {
            salon.LoadFromJson("cars.json");
            RefreshGrid();
            MessageBox.Show("Завантажено!");
        }

        // ПІДБІР
        private void btnFind_Click(object sender, EventArgs e)
        {
            var buyer = new Buyer
            {
                DesiredBrand = (Brand)comboBrand.SelectedItem,
                MaxPrice = decimal.Parse(txtPrice.Text),
                DesiredParts = new Dictionary<Part, Condition>
                {
                    { Part.Engine, Condition.Good }
                }
            };

            var selector = new AdvancedSelector();
            var matches = salon.FindMatches(buyer, selector);

            listBox1.Items.Clear();

            foreach (var car in matches)
            {
                listBox1.Items.Add(car.GetInfo());
            }

            if (matches.Count == 0)
            {
                listBox1.Items.Add("Нічого не знайдено → створено заявку");
            }
        }
    }
}
