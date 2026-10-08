using PassengerProfile.Models;
using PassengerProfile.ReportViewers;
using PassengerProfile.Repository;
using PassengerProfile.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PassengerProfile
{
    public partial class Form1 : Form
    {
        private readonly OpenFileDialog ofd = new OpenFileDialog();
        private bool isDefaultImage = true;
        private string previousImage = "";
        private int intPassengerId = 0;
        PassengerRepo repo = new PassengerRepo();
        private Passenger passenger = new Passenger();
        private List<SeatFacility> seatFacilities = new List<SeatFacility>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSeatTypeCombo();
            LoaddgvPassengerInfo();
            ClearAll();
        }

        private void LoaddgvPassengerInfo()
        {
            DataTable dt = repo.GetAllPassengers();
            if (!dt.Columns.Contains("Image"))
                dt.Columns.Add("Image", typeof(byte[]));
            foreach (DataRow dr in dt.Rows)
            {
                string imgName = dr["ImagePath"].ToString();
                string imgPath = Path.Combine(Application.StartupPath, "images", imgName);
                string defaultPath = Path.Combine(Application.StartupPath, "images", "no-image.png");
                try
                {
                    dr["Image"] = File.Exists(imgPath) ? File.ReadAllBytes(imgPath) : File.ReadAllBytes(defaultPath);
                }
                catch (Exception)
                {

                    dr["Image"] = null;
                }
            }
            dgvPassengerInfo.DataSource = null;
            dgvPassengerInfo.Columns.Clear();
            dgvPassengerInfo.DataSource = dt;
            AddGridButton(dgvPassengerInfo, "Detail", "Detail");
            dgvPassengerInfo.Columns["Detail"].DisplayIndex = 0;
            AddGridButton(dgvPassengerInfo, "Edit", "Edit");
            dgvPassengerInfo.Columns["Edit"].DisplayIndex = 1;
            AddGridButton(dgvPassengerInfo, "Delete", "Delete");
            dgvPassengerInfo.Columns["Delete"].DisplayIndex = 2;
            dgvPassengerInfo.RowTemplate.Height = 100;
            foreach (DataGridViewRow row in dgvPassengerInfo.Rows)
            {
                row.Height = 50;
            }
            if (dgvPassengerInfo.Columns["Image"] is DataGridViewImageColumn imgCol)
            {
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imgCol.Width = 50;
                imgCol.DisplayIndex = 3;
            }
            if (dgvPassengerInfo.Columns.Contains("PassengerId"))
            {
                dgvPassengerInfo.Columns["PassengerId"].Visible = false;
            }
            if (dgvPassengerInfo.Columns.Contains("SeatTypeId"))
            {
                dgvPassengerInfo.Columns["SeatTypeId"].Visible = false;
            }
            if (dgvPassengerInfo.Columns.Contains("ImagePath"))
            {
                dgvPassengerInfo.Columns["ImagePath"].Visible = false;
            }

        }

        private void AddGridButton(DataGridView grid, string name, string text)
        {
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn
            {
                Name = name,
                Text = text,
                HeaderText = text,
                Width = 60,
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(btn);
        }

        private void LoadSeatTypeCombo()
        {
            DataTable dt = repo.GetAllSeatTypes();
            DataRow topRow = dt.NewRow();
            topRow[0] = 0;
            topRow[1] = "  --Select--";
            dt.Rows.InsertAt(topRow, 0);
            cmbSeat.DataSource = dt;
            cmbSeat.DisplayMember = "SeatTypeName";
            cmbSeat.ValueMember = "SeatTypeId";
        }

        private void ClearAll()
        {

            string noImagePath = Path.Combine(Application.StartupPath, "images", "no-image.png");
            if (File.Exists(noImagePath))
            {
                using (var stream = new FileStream(noImagePath, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                }
            }
            isDefaultImage = true;
            previousImage = "";
            chkIsTransit.Checked = false;
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtMobileNo.Text = "";
            dgvNewFaclities.DataSource = null;
            dgvExistingFacilities.DataSource = null;
            btnSave.Text = "Save";
            intPassengerId = 0;
            passenger = new Passenger();
            txtBookFee.Text = "0";
            cmbSeat.SelectedIndex = 0;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                    isDefaultImage = false;
                    previousImage = "";
                }
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            using (var stream = new FileStream(Application.StartupPath + "\\images\\noimage.jpg", FileMode.Open, FileAccess.Read))
            {
                pbUpload.Image = Image.FromStream(stream);
                isDefaultImage = true;
                previousImage = "";
            }
        }

        private void dgvPassengerInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                int pId = Convert.ToInt32(dgvPassengerInfo.Rows[e.RowIndex].Cells["PassengerId"].Value);
                string command = dgvPassengerInfo.Columns[e.ColumnIndex].Name;
                switch (command)
                {
                    case "Detail":
                        dgvExistingFacilities.DataSource = repo.GetFacilitiesByPassengerId(pId);
                        if (dgvExistingFacilities.Columns.Contains("PassengerId"))
                        {
                            dgvExistingFacilities.Columns["PassengerId"].Visible = false;
                        }
                        if (dgvExistingFacilities.Columns.Contains("SeatFacilityId"))
                        {
                            dgvExistingFacilities.Columns["SeatFacilityId"].Visible = false;
                        }
                        break;
                    case "Delete":
                        if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            DeletePassengerInfo(pId);
                        }
                        break;
                    case "Edit":
                        EditPassengerInfo(pId);
                        break;
                    default:
                        MessageBox.Show("Invalid Operation");
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while processing the request: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditPassengerInfo(int pId)
        {
            DataTable dt = repo.GetPassengerByPassengerId(pId);
            if (dt == null || dt.Rows.Count == 0) return;
            btnSave.Text = "Update";
            intPassengerId = pId;
            DataRow row = dt.Rows[0];
            txtFirstName.Text = row["FisrtName"].ToString();
            txtLastName.Text = row["LastName"].ToString();
            txtMobileNo.Text = row["MobileNo"].ToString();
            dtpBookingDate.Value = Convert.ToDateTime(row["BookingDate"].ToString());
            chkIsTransit.Checked = row["IsTransit"] != DBNull.Value && (bool)row["IsTransit"];
            txtBookFee.Text = Convert.ToDecimal(row["BasicPrise"]).ToString();
            previousImage = row["ImagePath"].ToString();
            string imgpath = Path.Combine(Application.StartupPath, "images", previousImage);
            if (File.Exists(imgpath))
            {
                using (var stream = new FileStream(imgpath, FileMode.Open, FileAccess.Read))
                {
                    pbUpload.Image = Image.FromStream(stream);
                    isDefaultImage = false;
                }
            }
            else
            {
                isDefaultImage = true;
            }
            cmbSeat.SelectedValue = row["SeatTypeId"];
            passenger.Facilities = ConvertDataTableToSeatFacilities(pId);
            LoaddgvNewFacilities(passenger.Facilities, pId);
        }

        private void LoaddgvNewFacilities(List<SeatFacility> facilities, int passengerId)
        {
            dgvNewFaclities.DataSource = null;
            dgvNewFaclities.Columns.Clear();
            if (passengerId == 0)
                dgvNewFaclities.DataSource = ConvertToDataTable(facilities);
            else
                dgvNewFaclities.DataSource = repo.GetFacilitiesByPassengerId(passengerId);

            AddGridButton(dgvNewFaclities, "Delete", "Delete");
            dgvNewFaclities.Columns["Delete"].DisplayIndex = 0;

            if (dgvNewFaclities.Columns.Contains("PassengerId"))
            {
                dgvNewFaclities.Columns["PassengerId"].Visible = false;
            }
            if (dgvNewFaclities.Columns.Contains("SeatFacilityId"))
            {
                dgvNewFaclities.Columns["SeatFacilityId"].Visible = false;
            }
        }

        private DataTable ConvertToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) dataTable.Columns.Add(prop.Name);
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++) values[i] = Props[i].GetValue(item, null);
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        private List<SeatFacility> ConvertDataTableToSeatFacilities(int passengerId)
        {
            List<SeatFacility> list = new List<SeatFacility>();
            DataTable dt = repo.GetFacilitiesByPassengerId(passengerId);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SeatFacility
                {
                    SeatFacilityId = Convert.ToInt32(row["SeatFacilityId"]),
                    FacilityName = row["FacilityName"].ToString(),
                    PassengerId = passengerId,
                    Price = Convert.ToDecimal(row["Price"])
                });
            }
            return list;
        }

        private void DeletePassengerInfo(int pId)
        {
            int delete = repo.DeletePassengerByPassengerId(pId);
            LoaddgvPassengerInfo();
        }

        private void btnAddFacilities_Click(object sender, EventArgs e)
        {
            SeatFacility facility = new SeatFacility
            {
                FacilityName = txtFacility.Text,
                Price = Convert.ToDecimal(txtPrice.Text)
            };
            passenger.Facilities.Add(facility);
            LoaddgvNewFacilities(passenger.Facilities, 0);
            ClearFacilityInputField();
        }

        private void ClearFacilityInputField()
        {
            txtFacility.Text = "";
            txtPrice.Text = "";
        }

        private void dgvNewFaclities_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvNewFaclities.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        int pId = Convert.ToInt32(dgvNewFaclities.Rows[e.RowIndex].Cells["PassengerId"].Value);
                        int sfId = Convert.ToInt32(dgvNewFaclities.Rows[e.RowIndex].Cells["SeatFacilityId"].Value);
                        if (pId > 0 && sfId > 0)
                        {
                            repo.DeleteSeatFacilitiesByPassengerId(pId, sfId);
                        }
                    }
                    catch
                    {

                    }
                    if (passenger.Facilities.Count > e.RowIndex) passenger.Facilities.RemoveAt(e.RowIndex);
                    LoaddgvNewFacilities(passenger.Facilities, 0);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValidated())
            {
                MessageBox.Show("Provide correct Information", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                passenger.PassengerId = intPassengerId;
                passenger.FisrtName = txtFirstName.Text;
                passenger.LastName = txtLastName.Text;
                passenger.IsTransit = chkIsTransit.Checked;
                passenger.BookingDate = dtpBookingDate.Value;
                passenger.SeatTypeId = Convert.ToInt32(cmbSeat.SelectedValue);
                passenger.BasicPrise = Convert.ToDecimal(txtBookFee.Text);
                passenger.MobileNo = txtMobileNo.Text;
                if (isDefaultImage)
                {
                    passenger.ImagePath = "no-image.png";
                }
                else if (!string.IsNullOrEmpty(ofd.FileName) && previousImage == "")
                {
                    if (intPassengerId > 0 && previousImage != "" && previousImage != "no-image.png")
                    {
                        //DeleteImage(previousImage);
                    }
                    passenger.ImagePath = SaveImage(ofd.FileName);
                    previousImage = "";
                }

                if (intPassengerId == 0)
                {
                    if (repo.SavePassenger(passenger) > 0)
                    {
                        MessageBox.Show("Saved Successfully!");
                    }

                }
                else
                {
                    if (previousImage != "no-image.png" && previousImage != "")
                    {
                        passenger.ImagePath = previousImage;
                    }
                    if (repo.UpdatePassenger(passenger) > 0)
                    {
                        MessageBox.Show("Updated Successfully!");
                    }
                }

                btnSave.Text = "Update";
                intPassengerId = 0;

                LoaddgvPassengerInfo();
                ClearAll();

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteImage(string previousImage)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, "images", previousImage);
                if(File.Exists(path) && previousImage != "no-image.png")
                {
                    pbUpload.Image?.Dispose();
                    pbUpload.Image = null;
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Image delete Error: " + ex.Message);
            }
        }

        private string SaveImage(string imgPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imgPath);
            string ext = Path.GetExtension(imgPath);
            fileName = fileName.Length <= 15 ? fileName : fileName.Substring(0, 15);
            fileName = fileName + ext;
            string directoryPath = Path.Combine(Application.StartupPath, "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fullSavePath = Path.Combine(directoryPath, fileName);
            using (Bitmap btp = new Bitmap(pbUpload.Image))
            {
                btp.Save(fullSavePath);
            }
            return fileName;
        }

        private bool IsValidated()
        {
            return !string.IsNullOrWhiteSpace(txtFirstName.Text) && !string.IsNullOrWhiteSpace(txtLastName.Text) && !string.IsNullOrWhiteSpace(txtMobileNo.Text);
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            List<PassengerViewModel> list = new List<PassengerViewModel>();
            PassengerViewModel obj;
            DataTable dt = repo.GetAllPassengers();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    obj = new PassengerViewModel();
                    obj.PassengerId = Convert.ToInt32(dt.Rows[i]["PassengerId"]);
                    obj.FisrtName = dt.Rows[i]["FisrtName"].ToString();
                    obj.LastName = dt.Rows[i]["LastName"].ToString();
                    obj.MobileNo = dt.Rows[i]["MobileNo"].ToString();
                    obj.BookingDate = Convert.ToDateTime(dt.Rows[i]["BookingDate"].ToString());
                    //obj.IsTransit = Convert.ToBoolean(dt.Rows[i]["IsTransit"].ToString());
                    obj.SeatTypeName = dt.Rows[i]["SeatTypeName"].ToString();
                    obj.BasicPrise = Convert.ToDecimal(dt.Rows[i]["BasicPrise"].ToString());
                    obj.ImagePath = dt.Rows[i]["Imagepath"].ToString();
                    string fullPath = Application.StartupPath + "\\images\\" + obj.ImagePath;
                    if (System.IO.File.Exists(fullPath))
                    {
                        obj.ImageBinary = System.IO.File.ReadAllBytes(fullPath);
                    }
                    list.Add(obj);
                }
            }
            using (PassengerRptViewer rptViewer = new PassengerRptViewer(list))
            {
                rptViewer.ShowDialog();
            }
        }
    }
}
