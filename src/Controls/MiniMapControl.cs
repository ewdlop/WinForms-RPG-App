using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Localization;

namespace WinFormsApp1.Controls
{
    public partial class MiniMapControl : UserControl
    {
        private Panel mapPanel;
        private Button openFullMapButton;
        private Label locationLabel;
        private Label statusLabel;
        private Dictionary<string, Point> locationPositions;
        private string currentLocation;
        private HashSet<string> visitedLocations;

        public event EventHandler FullMapRequested;
        public event EventHandler<string> LocationClicked;

        public MiniMapControl()
        {
            locationPositions = new Dictionary<string, Point>();
            visitedLocations = new HashSet<string>();
            InitializeComponent();
            InitializeLocationPositions();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(200, 150);
            this.BackColor = Color.DarkGreen;
            this.BorderStyle = BorderStyle.FixedSingle;

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(3)
            };

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Location label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F)); // Map area
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Button

            // Current location label
            locationLabel = new Label
            {
                Text = LocalizationManager.GetString("MiniMap_Current", LocalizationManager.GetString("Status_Unknown")),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.DarkSlateGray,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Height = 20
            };
            mainLayout.Controls.Add(locationLabel, 0, 0);

            // Map display panel
            CreateMapPanel();
            mainLayout.Controls.Add(mapPanel, 0, 1);

            // Status label
            statusLabel = new Label
            {
                Text = LocalizationManager.GetString("MiniMap_ClickToTravel"),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.LightGreen,
                Font = new Font("Arial", 7),
                Height = 15
            };
            mainLayout.Controls.Add(statusLabel, 0, 2);

            // Open full map button
            openFullMapButton = new Button
            {
                Text = LocalizationManager.GetString("MiniMap_OpenFullMap"),
                Dock = DockStyle.Fill,
                Height = 25,
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            openFullMapButton.Click += OpenFullMapButton_Click;
            mainLayout.Controls.Add(openFullMapButton, 0, 3);

            this.Controls.Add(mainLayout);
        }

        private void CreateMapPanel()
        {
            mapPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.DarkSeaGreen,
                BorderStyle = BorderStyle.Fixed3D
            };
            mapPanel.Paint += MapPanel_Paint;
            mapPanel.MouseClick += MapPanel_MouseClick;
            mapPanel.MouseMove += MapPanel_MouseMove;

            // Add tooltip with localized text
            var toolTip = new ToolTip();
            var tooltipText = LocalizationManager.GetCurrentCulture().Name switch
            {
                "zh-CN" => "迷你世界地图 - 点击地点获取旅行信息",
                "zh-TW" => "迷你世界地圖 - 點擊地點獲取旅行資訊",
                "ja-JP" => "ミニワールドマップ - 場所をクリックして旅行情報を取得",
                "ko-KR" => "미니 월드맵 - 위치를 클릭하여 여행 정보 보기",
                "es-ES" => "Mini mapa del mundo - Haga clic en las ubicaciones para obtener información de viaje",
                "fr-FR" => "Mini carte du monde - Cliquez sur les emplacements pour obtenir des informations de voyage",
                "de-DE" => "Mini-Weltkarte - Klicken Sie auf Standorte, um Reiseinformationen zu erhalten",
                "ru-RU" => "Мини-карта мира - Нажмите на локации, чтобы получить информацию о путешествии",
                _ => "Mini world map - Click locations to get travel info"
            };
            toolTip.SetToolTip(mapPanel, tooltipText);
        }

        private void InitializeLocationPositions()
        {
            // Define relative positions for locations on the mini-map
            locationPositions = new Dictionary<string, Point>
            {
                ["village"] = new Point(50, 80),      // Bottom center (starting point)
                ["forest"] = new Point(30, 40),       // Upper left
                ["plains"] = new Point(70, 60),       // Right center
                ["cave"] = new Point(20, 20),         // Top left
                ["ruins"] = new Point(80, 30),        // Top right
                ["lair"] = new Point(50, 10)          // Top center (final boss)
            };
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw background terrain
            DrawTerrain(g);

            // Draw connections between locations
            DrawConnections(g);

            // Draw location markers
            DrawLocations(g);

            // Draw legend
            DrawLegend(g);
        }

        private void DrawTerrain(Graphics g)
        {
            // Draw simple terrain background
            using (var forestBrush = new SolidBrush(Color.DarkGreen))
            using (var plainsBrush = new SolidBrush(Color.YellowGreen))
            using (var mountainBrush = new SolidBrush(Color.Gray))
            {
                // Forest area (left side)
                g.FillEllipse(forestBrush, 10, 30, 40, 40);
                
                // Plains area (right side)
                g.FillEllipse(plainsBrush, 60, 50, 35, 35);
                
                // Mountain area (top)
                g.FillEllipse(mountainBrush, 40, 5, 30, 25);
            }
        }

        private void DrawConnections(Graphics g)
        {
            using (var connectionPen = new Pen(Color.Brown, 1))
            {
                connectionPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                // Draw paths between connected locations
                var connections = new[]
                {
                    ("village", "forest"),
                    ("village", "plains"),
                    ("forest", "cave"),
                    ("plains", "ruins"),
                    ("cave", "lair"),
                    ("ruins", "lair")
                };

                foreach (var (from, to) in connections)
                {
                    if (locationPositions.ContainsKey(from) && locationPositions.ContainsKey(to))
                    {
                        var fromPoint = locationPositions[from];
                        var toPoint = locationPositions[to];
                        g.DrawLine(connectionPen, fromPoint, toPoint);
                    }
                }
            }
        }

        private void DrawLocations(Graphics g)
        {
            foreach (var location in locationPositions)
            {
                string locationKey = location.Key;
                Point position = location.Value;
                
                // Determine location color and size based on status
                Color locationColor;
                int size;
                
                if (locationKey == currentLocation)
                {
                    locationColor = Color.Red;      // Current location
                    size = 8;
                }
                else if (visitedLocations.Contains(locationKey))
                {
                    locationColor = Color.Blue;     // Visited location
                    size = 6;
                }
                else
                {
                    locationColor = Color.Gray;     // Unvisited location
                    size = 5;
                }

                // Draw location marker
                using (var locationBrush = new SolidBrush(locationColor))
                using (var borderPen = new Pen(Color.Black, 1))
                {
                    Rectangle rect = new Rectangle(position.X - size/2, position.Y - size/2, size, size);
                    g.FillEllipse(locationBrush, rect);
                    g.DrawEllipse(borderPen, rect);
                }

                // Draw location name (abbreviated)
                string displayName = GetLocationDisplayName(locationKey);
                using (var textBrush = new SolidBrush(Color.White))
                using (var font = new Font("Arial", 6, FontStyle.Bold))
                {
                    var textSize = g.MeasureString(displayName, font);
                    var textPosition = new PointF(
                        position.X - textSize.Width / 2,
                        position.Y + size/2 + 2
                    );
                    
                    // Draw text background for readability
                    using (var bgBrush = new SolidBrush(Color.FromArgb(128, Color.Black)))
                    {
                        g.FillRectangle(bgBrush, textPosition.X - 1, textPosition.Y - 1, 
                            textSize.Width + 2, textSize.Height + 2);
                    }
                    
                    g.DrawString(displayName, font, textBrush, textPosition);
                }
            }
        }

        private void DrawLegend(Graphics g)
        {
            // Draw mini legend in corner
            int legendX = mapPanel.Width - 35;
            int legendY = 5;
            
            using (var font = new Font("Arial", 5))
            using (var textBrush = new SolidBrush(Color.White))
            using (var bgBrush = new SolidBrush(Color.FromArgb(180, Color.Black)))
            {
                // Background
                g.FillRectangle(bgBrush, legendX - 2, legendY - 2, 32, 25);
                
                // Legend items with localized text
                g.FillEllipse(Brushes.Red, legendX, legendY, 4, 4);
                g.DrawString(LocalizationManager.GetString("MiniMap_You"), font, textBrush, legendX + 6, legendY - 1);
                
                g.FillEllipse(Brushes.Blue, legendX, legendY + 6, 4, 4);
                g.DrawString(LocalizationManager.GetString("MiniMap_Visited"), font, textBrush, legendX + 6, legendY + 5);
                
                g.FillEllipse(Brushes.Gray, legendX, legendY + 12, 4, 4);
                g.DrawString(LocalizationManager.GetString("MiniMap_Unknown"), font, textBrush, legendX + 6, legendY + 11);
            }
        }

        private string GetLocationDisplayName(string locationKey)
        {
            // Use localized location names based on current culture
            return LocalizationManager.GetCurrentCulture().Name switch
            {
                "zh-CN" => locationKey switch
                {
                    "village" => "村庄",
                    "forest" => "森林",
                    "plains" => "平原",
                    "cave" => "洞穴",
                    "ruins" => "废墟",
                    "lair" => "巢穴",
                    _ => locationKey
                },
                "zh-TW" => locationKey switch
                {
                    "village" => "村莊",
                    "forest" => "森林",
                    "plains" => "平原",
                    "cave" => "洞穴",
                    "ruins" => "廢墟",
                    "lair" => "巢穴",
                    _ => locationKey
                },
                "ja-JP" => locationKey switch
                {
                    "village" => "村",
                    "forest" => "森",
                    "plains" => "平原",
                    "cave" => "洞窟",
                    "ruins" => "遺跡",
                    "lair" => "巣",
                    _ => locationKey
                },
                "ko-KR" => locationKey switch
                {
                    "village" => "마을",
                    "forest" => "숲",
                    "plains" => "평원",
                    "cave" => "동굴",
                    "ruins" => "유적",
                    "lair" => "소굴",
                    _ => locationKey
                },
                "es-ES" => locationKey switch
                {
                    "village" => "Aldea",
                    "forest" => "Bosque",
                    "plains" => "Llanuras",
                    "cave" => "Cueva",
                    "ruins" => "Ruinas",
                    "lair" => "Guarida",
                    _ => locationKey
                },
                "fr-FR" => locationKey switch
                {
                    "village" => "Village",
                    "forest" => "Forêt",
                    "plains" => "Plaines",
                    "cave" => "Grotte",
                    "ruins" => "Ruines",
                    "lair" => "Tanière",
                    _ => locationKey
                },
                "de-DE" => locationKey switch
                {
                    "village" => "Dorf",
                    "forest" => "Wald",
                    "plains" => "Ebenen",
                    "cave" => "Höhle",
                    "ruins" => "Ruinen",
                    "lair" => "Versteck",
                    _ => locationKey
                },
                "ru-RU" => locationKey switch
                {
                    "village" => "Деревня",
                    "forest" => "Лес",
                    "plains" => "Равнины",
                    "cave" => "Пещера",
                    "ruins" => "Руины",
                    "lair" => "Логово",
                    _ => locationKey
                },
                _ => locationKey switch
                {
                    "village" => "Village",
                    "forest" => "Forest",
                    "plains" => "Plains",
                    "cave" => "Cave",
                    "ruins" => "Ruins",
                    "lair" => "Lair",
                    _ => locationKey
                }
            };
        }

        private void MapPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // Find clicked location
            string clickedLocation = GetLocationAtPoint(e.Location);
            if (!string.IsNullOrEmpty(clickedLocation))
            {
                LocationClicked?.Invoke(this, clickedLocation);
                statusLabel.Text = LocalizationManager.GetString("MiniMap_Selected", GetLocationDisplayName(clickedLocation));
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // Show location name on hover
            string hoveredLocation = GetLocationAtPoint(e.Location);
            if (!string.IsNullOrEmpty(hoveredLocation))
            {
                statusLabel.Text = LocalizationManager.GetString("MiniMap_Hover", GetLocationDisplayName(hoveredLocation));
                mapPanel.Cursor = Cursors.Hand;
            }
            else
            {
                statusLabel.Text = LocalizationManager.GetString("MiniMap_ClickToTravel");
                mapPanel.Cursor = Cursors.Default;
            }
        }

        private string GetLocationAtPoint(Point clickPoint)
        {
            foreach (var location in locationPositions)
            {
                Point locationPoint = location.Value;
                int distance = (int)Math.Sqrt(
                    Math.Pow(clickPoint.X - locationPoint.X, 2) + 
                    Math.Pow(clickPoint.Y - locationPoint.Y, 2)
                );
                
                if (distance <= 8) // Click tolerance
                {
                    return location.Key;
                }
            }
            return null;
        }

        private void OpenFullMapButton_Click(object sender, EventArgs e)
        {
            FullMapRequested?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateCurrentLocation(string location)
        {
            if (!string.IsNullOrEmpty(currentLocation))
            {
                visitedLocations.Add(currentLocation);
            }
            
            currentLocation = location;
            visitedLocations.Add(location);
            
            locationLabel.Text = LocalizationManager.GetString("MiniMap_Current", GetLocationDisplayName(location));
            mapPanel.Invalidate(); // Trigger repaint
        }

        public void AddVisitedLocation(string location)
        {
            visitedLocations.Add(location);
            mapPanel.Invalidate();
        }

        public void ClearVisitedLocations()
        {
            visitedLocations.Clear();
            currentLocation = null;
            locationLabel.Text = LocalizationManager.GetString("MiniMap_Current", LocalizationManager.GetString("Status_Unknown"));
            mapPanel.Invalidate();
        }

        public void SetMapEnabled(bool enabled)
        {
            openFullMapButton.Enabled = enabled;
            mapPanel.Enabled = enabled;
        }

        /// <summary>
        /// Updates the control's text when language changes
        /// </summary>
        public void RefreshLocalization()
        {
            openFullMapButton.Text = LocalizationManager.GetString("MiniMap_OpenFullMap");
            statusLabel.Text = LocalizationManager.GetString("MiniMap_ClickToTravel");
            
            if (!string.IsNullOrEmpty(currentLocation))
            {
                locationLabel.Text = LocalizationManager.GetString("MiniMap_Current", GetLocationDisplayName(currentLocation));
            }
            else
            {
                locationLabel.Text = LocalizationManager.GetString("MiniMap_Current", LocalizationManager.GetString("Status_Unknown"));
            }
            
            mapPanel.Invalidate(); // Refresh legend and location names
        }
    }
} 