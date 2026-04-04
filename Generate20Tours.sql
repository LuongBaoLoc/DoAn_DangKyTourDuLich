USE [DoAn_DangKyTourDuLich];
GO

-- 1. DỌN DẸP DỮ LIỆU CŨ (Tránh lỗi Primary Key và Foreign Key)
DELETE FROM [OrderDetails];
DELETE FROM [Orders];
DELETE FROM [Reviews];
DELETE FROM [Tours];
DBCC CHECKIDENT ('Tours', RESEED, 0);

-- 2. NẠP FULL 15 TỈNH X 5 TOUR = 75 TOURS CHI TIẾT
INSERT INTO [Tours] 
(
    [Name], [ShortDescription], [DetailDescription], [Price], 
    [DepartureLocation], [Destination], [Duration], [DepartureDate], 
    [MaxParticipants], [CurrentParticipants], [ImageUrl], [Transportation], 
    [IsActive], [IsFeatured], [CreatedAt], [CategoryId], [Schedule]
)
VALUES 
-- =========================================================
-- 1. KIÊN GIANG (PHÚ QUỐC)
-- =========================================================
(N'Phú Quốc: Nghỉ dưỡng Đảo Ngọc', N'Vui chơi VinWonders & Safari.', N'Tour nghỉ dưỡng cao cấp.', 5500000, N'TP.HCM', N'Kiên Giang', 3, '2026-05-10', 30, 0, '/images/tours/pq1.jpg', N'Máy bay', 1, 1, GETDATE(), 1, 
N'Sáng: Check-in Grand World - Xem show Tinh hoa Việt Nam | Trưa: Thưởng thức Bún Quậy Kiến Xây | Chiều: Vui chơi VinWonders - Thủy cung Kim Quy | Tối: Cafe tại Sunset Sanato ngắm hoàng hôn'),
(N'Phú Quốc: Tour 4 Đảo Cano', N'Lặn ngắm san hô & Check-in hòn Thơm.', N'Tour mạo hiểm biển đảo.', 1800000, N'Phú Quốc', N'Kiên Giang', 1, '2026-05-12', 20, 0, '/images/tours/pq2.jpg', N'Cano', 1, 0, GETDATE(), 3, 
N'Sáng: Cano đi Hòn Mây Rút - Lặn ngắm san hô | Trưa: Ăn hải sản bè nổi Hòn Móng Tay | Chiều: Check-in Cầu Hôn (Kiss Bridge) | Tối: Cafe Chuồn Chuồn Bistro view toàn đảo'),
(N'Phú Quốc: Văn hóa & Tâm linh', N'Nhà tù Phú Quốc - Chùa Hộ Quốc.', N'Tìm hiểu lịch sử.', 1200000, N'Phú Quốc', N'Kiên Giang', 1, '2026-05-15', 30, 0, '/images/tours/pq3.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Viếng Chùa Hộ Quốc - Thăm Nhà tù Phú Quốc | Trưa: Ăn gỏi cá trích tại bãi Kem | Chiều: Thăm Vườn Tiêu - Cơ sở Ngọc Trai | Tối: Cafe OCSEN Beach Bar ngắm biển'),
(N'Phú Quốc: Food Tour Chợ Đêm', N'Ăn sập hải sản & Ăn vặt.', N'Thiên đường ẩm thực.', 950000, N'Phú Quốc', N'Kiên Giang', 1, '2026-05-18', 15, 0, '/images/tours/pq4.jpg', N'Xe điện', 1, 0, GETDATE(), 5, 
N'Chiều: Thăm Sunset Town phong cách Ý | Tối: Thưởng thức hải sản nướng Chợ Đêm - Kem cuộn - Cafe Phố Biển'),
(N'Phú Quốc: Team Building Bãi Sao', N'Gắn kết tập thể trên cát trắng.', N'Sôi động hè rực rỡ.', 2500000, N'TP.HCM', N'Kiên Giang', 2, '2026-05-20', 100, 0, '/images/tours/pq5.jpg', N'Máy bay', 1, 0, GETDATE(), 3, 
N'Ngày 1: Team Building bãi biển - Gala Dinner | Ngày 2: Cafe Starbucks Địa Trung Hải - Mua sắm đặc sản'),

-- =========================================================
-- 2. ĐÀ NẴNG
-- =========================================================
(N'Đà Nẵng: Siêu phẩm Bà Nà Hills', N'Check-in Cầu Vàng - Fantasy Park.', N'Nghỉ dưỡng & Giải trí.', 4200000, N'TP.HCM', N'Đà Nẵng', 3, '2026-05-01', 40, 0, '/images/tours/dn1.jpg', N'Máy bay', 1, 1, GETDATE(), 1, 
N'Sáng: Lên cáp treo Bà Nà - Check-in Cầu Vàng | Trưa: Buffet 100 món tại nhà hàng Arapang | Chiều: Vui chơi Fantasy Park - Thăm hầm rượu Debay | Tối: Cafe tại hầm rượu hoặc dạo Cầu Rồng'),
(N'Đà Nẵng: Trekking Rừng Sơn Trà', N'Khám phá Đỉnh Bàn Cờ - Linh Ứng.', N'Mạo hiểm thiên nhiên.', 1500000, N'Đà Nẵng', N'Đà Nẵng', 1, '2026-05-05', 15, 0, '/images/tours/dn2.jpg', N'Xe Jeep', 1, 0, GETDATE(), 4, 
N'Sáng: Leo núi Sơn Trà ngắm Voọc | Trưa: Ăn cá cu nướng đá tại rừng | Chiều: Viếng chùa Linh Ứng - Cafe Sơn Trà Marina | Tối: Dạo cầu Tình Yêu'),
(N'Đà Nẵng: Ngũ Hành Sơn - Hội An', N'Làng đá Non Nước - Phố cổ.', N'Di sản văn hóa.', 1300000, N'Đà Nẵng', N'Đà Nẵng', 1, '2026-05-10', 25, 0, '/images/tours/dn3.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Chiều: Tham quan Ngũ Hành Sơn | Tối: Dạo phố cổ Hội An - Thả đèn hoa đăng - Cafe Faifo ngắm view từ tầng cao'),
(N'Đà Nẵng: Food Tour Mì Quảng', N'Mì Quảng - Bánh xèo - Hải sản.', N'Hương vị miền Trung.', 800000, N'Đà Nẵng', N'Đà Nẵng', 1, '2026-05-12', 20, 0, '/images/tours/dn4.jpg', N'Xe điện', 1, 0, GETDATE(), 5, 
N'Trưa: Thưởng thức Mì Quảng Ếch | Chiều: Ăn bánh xèo bà Dưỡng | Tối: Đại tiệc hải sản Mỹ Khê - Cafe Wonderlust'),
(N'Đà Nẵng: Team Building Biển', N'Trò chơi vận động dưới nước.', N'Năng động & Gắn kết.', 1900000, N'Hà Nội', N'Đà Nẵng', 2, '2026-05-15', 200, 0, '/images/tours/dn5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'Ngày 1: Teambuilding bãi biển Mỹ Khê - Gala Dinner | Ngày 2: Check-in Công viên APEC - Cafe Highland du thuyền'),

-- =========================================================
-- 3. LÂM ĐỒNG (ĐÀ LẠT)
-- =========================================================
(N'Đà Lạt: Thiên đường Sống ảo', N'Check-in các vườn hoa & Cafe hot.', N'Nghỉ dưỡng nhẹ nhàng.', 3200000, N'TP.HCM', N'Lâm Đồng', 3, '2026-05-15', 25, 0, '/images/tours/dl1.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Vườn hoa Thành phố - Dinh Bảo Đại | Trưa: Ăn Lẩu gà lá é Tao Đàn | Chiều: Cafe Still Cafe phong cách Nhật | Tối: Dạo chợ đêm Đà Lạt ăn bánh tráng nướng'),
(N'Đà Lạt: Trekking Langbiang', N'Chinh phục đỉnh núi - Săn mây.', N'Mạo hiểm núi rừng.', 1500000, N'Đà Lạt', N'Lâm Đồng', 1, '2026-05-18', 15, 0, '/images/tours/dl2.jpg', N'Xe Jeep', 1, 0, GETDATE(), 4, 
N'Sáng: 04h00 Săn mây Cầu Đất - Cafe Túi Mơ To | Trưa: Picnic đỉnh Langbiang | Chiều: Trượt thác Datanla | Tối: Giao lưu cồng chiêng'),
(N'Đà Lạt: Văn hóa Ga & Chùa cổ', N'Ga Đà Lạt - Chùa Linh Phước.', N'Kiến trúc & Tâm linh.', 1200000, N'Đà Lạt', N'Lâm Đồng', 1, '2026-05-20', 30, 0, '/images/tours/dl3.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Ga Đà Lạt - Đi xe lửa cổ | Trưa: Ăn nem nướng Bà Hùng | Chiều: Viếng Chùa Linh Phước | Tối: Cafe Tùng ngắm phố xưa'),
(N'Đà Lạt: Food Tour Phố Núi', N'Bánh căn - Sữa đậu nành.', N'Ấm áp đêm Đà Lạt.', 750000, N'Đà Lạt', N'Lâm Đồng', 1, '2026-05-22', 20, 0, '/images/tours/dl4.jpg', N'Xe máy', 1, 0, GETDATE(), 5, 
N'Chiều: Ăn bánh căn Lệ | Tối: Sữa đậu nành Tăng Bạt Hổ - Cafe Lululola nghe nhạc Acoustic view thung lũng'),
(N'Đà Lạt: Team Building Rừng Thông', N'Trò chơi sinh tồn - Lửa trại.', N'Gắn kết đồng đội.', 1800000, N'TP.HCM', N'Lâm Đồng', 2, '2026-05-25', 100, 0, '/images/tours/dl5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'Ngày 1: Teambuilding Thung lũng Vàng - Lửa trại | Ngày 2: Check-in đường hầm Đất Sét - Cafe Horizon'),

-- =========================================================
-- 4. LÀO CAI (SAPA)
-- =========================================================
(N'Sapa: Chinh phục Fansipan', N'Cáp treo Fansipan - Bản Cát Cát.', N'Chinh phục đỉnh cao.', 3800000, N'Hà Nội', N'Lào Cai', 3, '2026-06-01', 25, 0, '/images/tours/sp1.jpg', N'Xe giường nằm', 1, 1, GETDATE(), 4, 
N'Sáng: Cáp treo Fansipan ngắm thung lũng | Trưa: Buffet trên núi | Chiều: Bản Cát Cát - Cafe Lá Đỏ view núi | Tối: Ăn thắng cố - Cafe Cộng Sapa'),
(N'Sapa: Food Tour vùng cao', N'Cơm lam, lợn bản, rượu ngô.', N'Hương vị Tây Bắc.', 1100000, N'Lào Cai', N'Lào Cai', 1, '2026-06-05', 20, 0, '/images/tours/sp2.jpg', N'Xe điện', 1, 0, GETDATE(), 5, 
N'Chiều: Thăm chợ Sapa | Tối: Ăn đồ nướng vỉa hè - Lợn cắp nách nướng - Cafe tại Sailing Sapa view biển mây'),
(N'Sapa: Nghỉ dưỡng Silk Path', N'Resort chuẩn châu Âu giữa mây.', N'Sang trọng & Lãng mạn.', 5200000, N'Hà Nội', N'Lào Cai', 2, '2026-06-10', 15, 0, '/images/tours/sp3.jpg', N'Limousine', 1, 1, GETDATE(), 1, 
N'Ngày 1: Check-in resort - Tắm lá thuốc dao đỏ | Ngày 2: Cafe Haven Sapa Camp Site - Tiễn khách'),
(N'Sapa: Trekking Lao Chải', N'Đi bộ xuyên ruộng bậc thang.', N'Văn hóa cộng đồng.', 1900000, N'Lào Cai', N'Lào Cai', 2, '2026-06-15', 12, 0, '/images/tours/sp4.jpg', N'Đi bộ', 1, 0, GETDATE(), 1, 
N'Sáng: Trekking qua Lao Chải - Tà Van | Trưa: Ăn tại Homestay dân tộc | Chiều: Tắm suối - Cafe tại bản của người H''Mong'),
(N'Sapa: Team Building Fansipan', N'Hoạt động gắn kết đỉnh núi.', N'Vượt qua giới hạn.', 2500000, N'Hà Nội', N'Lào Cai', 2, '2026-06-20', 80, 0, '/images/tours/sp5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'Ngày 1: Gala Lửa trại quảng trường | Ngày 2: Teambuilding chinh phục 600 bậc đá Fansipan'),

-- =========================================================
-- 5. QUẢNG NINH (HẠ LONG)
-- =========================================================
(N'Hạ Long: Du thuyền 5 sao', N'Ngủ đêm trên vịnh di sản.', N'Kỳ quan thiên nhiên.', 5500000, N'Hà Nội', N'Quảng Ninh', 2, '2026-07-01', 40, 0, '/images/tours/qn1.jpg', N'Du thuyền', 1, 1, GETDATE(), 3, 
N'Ngày 1: Lên tàu - Tiệc Sunset trên boong | Trưa: Ăn hải sản tươi sống | Chiều: Chèo Kayak hang Sửng Sốt | Tối: Câu mực đêm - Cafe tầng thượng tàu'),
(N'Hạ Long: Sun World & Onsen', N'Công viên rồng & Khoáng nóng.', N'Giải trí đỉnh cao.', 3900000, N'Hà Nội', N'Quảng Ninh', 3, '2026-07-05', 30, 0, '/images/tours/qn2.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Vui chơi Sun World Park | Trưa: Ăn bún bề bề Cái Dăm | Chiều: Tắm khoáng Yoko Onsen Nhật Bản | Tối: Cafe Old Town view vịnh'),
(N'Quảng Ninh: Food Tour Hải sản', N'Sá sùng, cù kỳ, bề bề.', N'Tinh túy vị biển.', 1200000, N'Quảng Ninh', N'Quảng Ninh', 1, '2026-07-10', 25, 0, '/images/tours/qn3.jpg', N'Xe điện', 1, 0, GETDATE(), 5, 
N'Trưa: Hải sản tươi sống tại Cảng | Tối: Chợ đêm Bãi Cháy - Ăn sữa chua trân châu Hạ Long - Cafe Rooftop 1900'),
(N'Hạ Long: Khám phá Đảo Mắt Rồng', N'Hồ nước giữa núi đá vôi.', N'Mạo hiểm - Hoang sơ.', 1800000, N'Quảng Ninh', N'Quảng Ninh', 1, '2026-07-15', 15, 0, '/images/tours/qn4.jpg', N'Cano', 1, 0, GETDATE(), 4, 
N'Sáng: Cano ra đảo hoang | Trưa: Picnic bãi biển | Chiều: Lặn ngắm san hô hoang sơ'),
(N'Hạ Long: Team Building Bãi Cháy', N'Sân chơi lớn gắn kết đồng đội.', N'Vận động hè rực rỡ.', 1500000, N'Hà Nội', N'Quảng Ninh', 2, '2026-07-20', 200, 0, '/images/tours/qn5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'Ngày 1: Teambuilding bãi biển - Gala Dinner | Ngày 2: Tham quan bảo tàng Quảng Ninh - Cafe Hải Đăng'),

-- =========================================================
-- 6. NINH BÌNH
-- =========================================================
(N'Ninh Bình: Tràng An - Hang Múa', N'Chèo thuyền & Leo núi.', N'Di sản kép thế giới.', 1500000, N'Hà Nội', N'Ninh Bình', 1, '2026-05-15', 50, 0, '/images/tours/nb1.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Chinh phục Hang Múa ngắm lúa chín | Trưa: Đặc sản thịt dê cơm cháy | Chiều: Thuyền Tràng An phim trường Kong | Tối: Cafe Phố cổ Hoa Lư'),
(N'Ninh Bình: Nghỉ dưỡng Emeralda', N'Làng quê Bắc Bộ thu nhỏ.', N'Yên bình - Xanh mát.', 3200000, N'Hà Nội', N'Ninh Bình', 2, '2026-05-18', 20, 0, '/images/tours/nb2.jpg', N'Limousine', 1, 1, GETDATE(), 1, 
N'Ngày 1: Check-in Resort - Đạp xe đồng quê | Ngày 2: Spa thảo mộc - Thăm Chùa Bái Đính - Cafe Chuồn Chuồn'),
(N'Ninh Bình: Food Tour Dê Núi', N'Thịt dê tái chanh, cơm cháy.', N'Ẩm thực cố đô.', 900000, N'Ninh Bình', N'Ninh Bình', 1, '2026-05-20', 30, 0, '/images/tours/nb3.jpg', N'Xe máy', 1, 0, GETDATE(), 5, 
N'Trưa: Đại tiệc Dê núi 7 món | Tối: Ốc núi luộc - Cafe ven sông Ngô Đồng ngắm thuyền về'),
(N'Ninh Bình: Trekking Rừng Cúc Phương', N'Khám phá hệ sinh thái rừng.', N'Mạo hiểm thiên nhiên.', 1400000, N'Hà Nội', N'Ninh Bình', 1, '2026-05-22', 20, 0, '/images/tours/nb4.jpg', N'Xe du lịch', 1, 0, GETDATE(), 4, 
N'Sáng: Khám phá động Người Xưa | Trưa: Cơm nắm muối mè | Chiều: Thăm trung tâm cứu hộ linh trưởng'),
(N'Ninh Bình: Team Building Thuyền Rồng', N'Đua thuyền trên dòng Sào Khê.', N'Giao lưu & Gắn kết.', 1200000, N'Hà Nội', N'Ninh Bình', 1, '2026-05-25', 100, 0, '/images/tours/nb5.jpg', N'Xe buýt', 1, 0, GETDATE(), 3, 
N'Sáng: Khởi động tại bãi Hoa Lư | Trưa: Buffet đặc sản | Chiều: Đua thuyền rồng - Trao giải'),

-- =========================================================
-- 7. THỪA THIÊN HUẾ
-- =========================================================
(N'Huế: Cố Đô Trầm Mặc', N'Đại Nội - Lăng Tẩm - Sông Hương.', N'Tìm về lịch sử.', 1200000, N'Huế', N'Thừa Thiên Huế', 1, '2026-06-01', 30, 0, '/images/tours/hue1.jpg', N'Xích lô', 1, 1, GETDATE(), 1, 
N'Sáng: Đại Nội - Chùa Thiên Mụ | Trưa: Cơm cung đình | Chiều: Lăng Khải Định - Tịnh Tâm Kim Cổ Cafe | Tối: Nghe Ca Huế trên sông Hương'),
(N'Huế: Food Tour Cơm Hến', N'Cơm hến, bánh bèo, bánh lọc.', N'Tinh túy ẩm thực Huế.', 700000, N'Huế', N'Thừa Thiên Huế', 1, '2026-06-03', 20, 0, '/images/tours/hue2.jpg', N'Xe đạp', 1, 0, GETDATE(), 5, 
N'Trưa: Cơm hến vỉa hè Cồn Hến | Chiều: Bánh nậm lọc bà Đỏ | Tối: Chè hẻm 20 món - Cafe tại Vy Dạ Vỹ Dạ Thôn'),
(N'Huế: Nghỉ dưỡng Vedana Lagoon', N'Khu nghỉ dưỡng trên đầm phá.', N'Yên bình tuyệt đối.', 4500000, N'TP.HCM', N'Thừa Thiên Huế', 3, '2026-06-05', 10, 0, '/images/tours/hue3.jpg', N'Máy bay', 1, 1, GETDATE(), 1, 
N'Ngày 1: Check-in Lagoon - Ngắm hoàng hôn | Ngày 2: Yoga sáng - Spa thiền | Ngày 3: Tiễn sân bay'),
(N'Huế: Trekking Bạch Mã', N'Hải Vọng Đài - Ngũ Hồ.', N'Chinh phục đỉnh cao.', 1800000, N'Đà Nẵng', N'Thừa Thiên Huế', 1, '2026-06-08', 15, 0, '/images/tours/hue4.jpg', N'Xe du lịch', 1, 0, GETDATE(), 4, 
N'Sáng: Leo đỉnh Bạch Mã | Trưa: Ăn trưa bên suối Ngũ Hồ | Chiều: Tắm hồ mát lạnh - Về lại'),
(N'Huế: Team Building Cố Đô', N'Mật thư lịch sử - Giải mã Đại Nội.', N'Trí tuệ & Đồng đội.', 1400000, N'Huế', N'Thừa Thiên Huế', 1, '2026-06-10', 60, 0, '/images/tours/hue5.jpg', N'Xe buýt', 1, 0, GETDATE(), 3, 
N'Sáng: Giải mã mật thư kinh thành | Trưa: Ăn món Huế tập thể | Chiều: Trò chơi dân gian'),

-- =========================================================
-- 8. QUẢNG NAM (HỘI AN)
-- =========================================================
(N'Hội An: Phố Cổ Lung Linh', N'Đèn lồng - Thả hoa đăng.', N'Lãng mạn - Hoài niệm.', 900000, N'Đà Nẵng', N'Quảng Nam', 1, '2026-06-15', 50, 0, '/images/tours/ha1.jpg', N'Đi bộ', 1, 1, GETDATE(), 1, 
N'Chiều: Thăm Chùa Cầu - Làng Gốm Thanh Hà | Tối: Thả đèn hoa đăng sông Hoài - Cafe Faifo view mái ngói | Tối muộn: Ăn Bánh mì Phượng'),
(N'Hội An: Food Tour Phố Hoài', N'Cao lầu - Bánh bao bánh vạc.', N'Hương vị đặc trưng.', 650000, N'Hội An', N'Quảng Nam', 1, '2026-06-18', 20, 0, '/images/tours/ha2.jpg', N'Xe đạp', 1, 0, GETDATE(), 5, 
N'Trưa: Cao lầu Thanh | Chiều: Bánh bao bánh vạc Bông Hồng Trắng | Tối: Chè bắp Cồn Hến - Cafe Reaching Out'),
(N'Hội An: Nghỉ dưỡng Vinpearl Nam Hội An', N'Vui chơi & Nghỉ dưỡng chuẩn 5 sao.', N'Hiện đại - Đẳng cấp.', 4800000, N'TP.HCM', N'Quảng Nam', 3, '2026-06-20', 30, 0, '/images/tours/ha3.jpg', N'Máy bay', 1, 1, GETDATE(), 1, 
N'Ngày 1: Đón khách - Check-in | Ngày 2: Full ngày VinWonders - River Safari | Ngày 3: Tiễn khách'),
(N'Hội An: Trekking Rừng dừa Bảy Mẫu', N'Múa thúng chai mạo hiểm.', N'Sông nước dân dã.', 850000, N'Hội An', N'Quảng Nam', 1, '2026-06-22', 40, 0, '/images/tours/ha4.jpg', N'Thuyền thúng', 1, 0, GETDATE(), 4, 
N'Sáng: Chèo thúng xuyên rừng dừa | Trưa: Ăn cá tai tượng chiên xù | Chiều: Học nấu ăn cùng dân bản'),
(N'Hội An: Team Building Bài Chòi', N'Hát bài chòi - Giao lưu văn hóa.', N'Nghệ thuật & Gắn kết.', 1000000, N'Hội An', N'Quảng Nam', 1, '2026-06-25', 100, 0, '/images/tours/ha5.jpg', N'Xe điện', 1, 0, GETDATE(), 3, 
N'Chiều: Tổ chức Team Building dạo bộ tìm hiểu lịch sử | Tối: Xem show Ký ức Hội An hoành tráng'),

-- =========================================================
-- 9. KHÁNH HÒA (NHA TRANG)
-- =========================================================
(N'Nha Trang: Nghỉ dưỡng Vinpearl', N'Đảo Hòn Tre - Thiên đường vui chơi.', N'Full dịch vụ.', 5200000, N'TP.HCM', N'Khánh Hòa', 3, '2026-07-01', 50, 0, '/images/tours/nt1.jpg', N'Máy bay', 1, 1, GETDATE(), 1, 
N'Ngày 1: Cáp treo ra đảo - Tắm biển | Ngày 2: VinWonders - Show Nhạc nước | Ngày 3: Tắm bùn khoáng nóng - Cafe Rain Forest'),
(N'Nha Trang: Lặn San Hô Hòn Mun', N'Khám phá lòng đại dương.', N'Mạo hiểm kỳ thú.', 1800000, N'Nha Trang', N'Khánh Hòa', 1, '2026-07-05', 12, 0, '/images/tours/nt2.jpg', N'Cano', 1, 0, GETDATE(), 4, 
N'Sáng: Lặn bình khí chuyên nghiệp | Trưa: Thưởng thức hải sản bè | Chiều: Tắm biển Bãi Tranh - Cafe Sailing Club'),
(N'Nha Trang: Food Tour Bún Cá', N'Bún cá sứa - Nem nướng.', N'Vị biển nồng nàn.', 800000, N'Nha Trang', N'Khánh Hòa', 1, '2026-07-08', 20, 0, '/images/tours/nt3.jpg', N'Xe điện', 1, 0, GETDATE(), 5, 
N'Sáng: Bún cá sứa Năm Beo | Trưa: Nem nướng Đặng Văn Quyên | Tối: Chợ đêm Nha Trang - Cafe An Café'),
(N'Khánh Hòa: Văn hóa Tháp Bà', N'Di tích Chăm Pa - Chùa Long Sơn.', N'Tâm linh - Nghệ thuật.', 1100000, N'Nha Trang', N'Khánh Hòa', 1, '2026-07-10', 40, 0, '/images/tours/nt4.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Tháp Bà Ponagar | Trưa: Ăn đặc sản gà chỉ | Chiều: Viện Hải Dương Học - Cafe view tháp trầm hương'),
(N'Nha Trang: Team Building Du Thuyền', N'Tiệc rượu vang - Nhạc sống.', N'Đẳng cấp & Gắn kết.', 2800000, N'Nha Trang', N'Khánh Hòa', 1, '2026-07-15', 60, 0, '/images/tours/nt5.jpg', N'Du thuyền', 1, 0, GETDATE(), 3, 
N'17h00: Lên du thuyền Sealife ngắm hoàng hôn | 19h00: Teambuilding nhẹ nhàng - Tiệc tối món Âu'),

-- =========================================================
-- 10. BÌNH THUẬN (MŨI NÉ)
-- =========================================================
(N'Mũi Né: Săn Hoàng Hôn Bàu Trắng', N'Đồi cát trắng - Xe Jeep.', N'Check-in cực hot.', 1500000, N'TP.HCM', N'Bình Thuận', 2, '2026-08-01', 30, 0, '/images/tours/mn1.jpg', N'Xe du lịch', 1, 1, GETDATE(), 4, 
N'Sáng: Săn bình minh đồi cát | Trưa: Ăn bánh xèo Phan Thiết | Chiều: Xe Jeep địa hình Bàu Trắng | Tối: Cafe Chameleon Beach Bar phong cách Thái'),
(N'Mũi Né: Nghỉ dưỡng Centara', N'Resort phong cách Địa Trung Hải.', N'Rực rỡ nắng hè.', 4200000, N'TP.HCM', N'Bình Thuận', 3, '2026-08-05', 20, 0, '/images/tours/mn2.jpg', N'Xe Limousine', 1, 1, GETDATE(), 1, 
N'Ngày 1: Check-in Resort - Tắm hồ bơi | Ngày 2: Lâu đài rượu vang - Ăn tối ven biển | Ngày 3: Tiễn khách'),
(N'Bình Thuận: Food Tour Răng Mực', N'Hải sản làng chài Mũi Né.', N'Món ngon dân dã.', 900000, N'Phan Thiết', N'Bình Thuận', 1, '2026-08-08', 25, 0, '/images/tours/mn3.jpg', N'Xe máy', 1, 0, GETDATE(), 5, 
N'Trưa: Lẩu thả đặc sản | Tối: Ăn vặt Răng mực Võ Thị Sáu - Cafe dừa tại Hanna Beach'),
(N'Phan Thiết: Văn hóa Tháp Chăm', N'Tháp Po Sah Inư - Trường Dục Thanh.', N'Lịch sử - Kiến trúc.', 1100000, N'Phan Thiết', N'Bình Thuận', 1, '2026-08-10', 40, 0, '/images/tours/mn4.jpg', N'Xe du lịch', 1, 0, GETDATE(), 1, 
N'Sáng: Tháp Po Sah Inư | Trưa: Ăn vịt thả | Chiều: Trường Dục Thanh - Bảo tàng Hồ Chí Minh'),
(N'Mũi Né: Team Building Đua Môtô Cát', N'Thử thách tốc độ đồng đội.', N'Kịch tính & Hấp dẫn.', 1900000, N'TP.HCM', N'Bình Thuận', 2, '2026-08-15', 100, 0, '/images/tours/mn5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'Ngày 1: Teambuilding Đồi cát hồng - Gala Dinner | Ngày 2: Tham quan Suối Tiên - Về lại'),

-- =========================================================
-- 11. TP. HỒ CHÍ MINH
-- =========================================================
(N'Sài Gòn: Ngắm sông từ Bitexco', N'Dinh Độc Lập - Bưu Điện - Landmark.', N'Sài Gòn hoa lệ.', 1100000, N'TP.HCM', N'TP. Hồ Chí Minh', 1, '2026-04-10', 40, 0, '/images/tours/hcm1.jpg', N'Xe buýt 2 tầng', 1, 1, GETDATE(), 1, 
N'Sáng: Dinh Độc Lập - Nhà Thờ Đức Bà | Trưa: Cơm tấm bãi rác Q4 | Chiều: Landmark 81 - Cafe Blanket & Roses'),
(N'TP.HCM: Mạo hiểm Địa đạo Củ Chi', N'Hành trình dưới lòng đất.', N'Khám phá quân sự.', 950000, N'TP.HCM', N'TP. Hồ Chí Minh', 1, '2026-04-12', 40, 0, '/images/tours/hcm2.jpg', N'Xe buýt', 1, 0, GETDATE(), 4, 
N'Sáng: Thăm địa đạo | Trưa: Ăn khoai mì chấm muối mè | Chiều: Bắn súng thể thao - Về'),
(N'Sài Gòn: Nghỉ dưỡng Riverside', N'Khách sạn 5 sao ven sông.', N'Sang trọng phố thị.', 3500000, N'TP.HCM', N'TP. Hồ Chí Minh', 2, '2026-04-15', 15, 0, '/images/tours/hcm3.jpg', N'Xe hơi', 1, 1, GETDATE(), 1, 
N'Ngày 1: Check-in - Ăn tối du thuyền Indochina Junk | Ngày 2: Cafe sáng dạo phố đi bộ - Tiễn khách'),
(N'TP.HCM: Food Tour Ốc Quận 4', N'Ăn sập các loại ốc vỉa hè.', N'Ẩm thực đêm Sài Gòn.', 700000, N'TP.HCM', N'TP. Hồ Chí Minh', 1, '2026-04-18', 25, 0, '/images/tours/hcm4.jpg', N'Xe máy', 1, 0, GETDATE(), 5, 
N'19h00: Ốc Đào | 20h30: Phá lấu dì Liên | 22h00: Bia hơi Bùi Viện - Cafe vợt Phan Đình Phùng'),
(N'TP.HCM: Team Building Bitexco', N'Giải mật thư quanh trung tâm.', N'Trí tuệ & Đồng đội.', 1200000, N'TP.HCM', N'TP. Hồ Chí Minh', 1, '2026-04-20', 100, 0, '/images/tours/hcm5.jpg', N'Xe buýt', 1, 0, GETDATE(), 3, 
N'Sáng: Teambuilding mật thư phố đi bộ | Trưa: Tiệc buffet | Chiều: Tham quan Bảo tàng Chứng tích Chiến tranh'),

-- =========================================================
-- 12. BÀ RỊA - VŨNG TÀU
-- =========================================================
(N'Vũng Tàu: Nghỉ dưỡng The Imperial', N'Khách sạn phong cách Victoria.', N'Lộng lẫy - Cổ điển.', 4500000, N'TP.HCM', N'Bà Rịa - Vũng Tàu', 2, '2026-05-01', 20, 0, '/images/tours/vt1.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Ngày 1: Đón khách - Tea party | Ngày 2: Tắm biển riêng - Thăm Bạch Dinh - Cafe Lightroom view biển'),
(N'Vũng Tàu: Trekking Núi Lớn mạo hiểm', N'Ngắm toàn cảnh biển từ đỉnh.', N'Thử thách thể lực.', 800000, N'Vũng Tàu', N'Bà Rịa - Vũng Tàu', 1, '2026-05-03', 30, 0, '/images/tours/vt2.jpg', N'Đi bộ', 1, 0, GETDATE(), 4, 
N'Sáng: Leo Núi Lớn | Trưa: Ăn bánh khọt Cô Ba | Chiều: Tượng Chúa Kitô - Cafe Mũi Đá'),
(N'Vũng Tàu: Food Tour Lẩu Cá Đuối', N'Bánh khọt - Lẩu cá đuối.', N'Đặc sản phố biển.', 650000, N'Vũng Tàu', N'Bà Rịa - Vũng Tàu', 1, '2026-05-05', 20, 0, '/images/tours/vt3.jpg', N'Xe điện', 1, 0, GETDATE(), 5, 
N'Trưa: Lẩu cá đuối Hoàng Minh | Tối: Kem Alibaba - Cafe Ô Cấp ngắm sóng'),
(N'Vũng Tàu: Văn hóa Ngọn Hải Đăng', N'Ngọn hải đăng cổ nhất Việt Nam.', N'Lịch sử - Kiến trúc.', 900000, N'Vũng Tàu', N'Bà Rịa - Vũng Tàu', 1, '2026-05-08', 40, 0, '/images/tours/vt4.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Sáng: Hải đăng Vũng Tàu | Trưa: Ăn cơm niêu | Chiều: Chùa Hộ Pháp - Thăm làng bè Long Sơn'),
(N'Vũng Tàu: Team Building Hồ Tràm', N'Vui chơi trên bãi biển hoang sơ.', N'Hoành tráng & Gắn kết.', 1900000, N'TP.HCM', N'Bà Rịa - Vũng Tàu', 2, '2026-05-10', 300, 0, '/images/tours/vt5.jpg', N'Xe buýt', 1, 0, GETDATE(), 3, 
N'Ngày 1: Team Building Hồ Tràm - Gala Diner | Ngày 2: Check-in rừng Bình Châu - Về'),

-- =========================================================
-- 13. CẦN THƠ
-- =========================================================
(N'Cần Thơ: Chợ Nổi & Miệt Vườn', N'Cái Răng - Mỹ Khánh - Sông nước.', N'Văn hóa miền Tây.', 1200000, N'TP.HCM', N'Cần Thơ', 2, '2026-06-01', 50, 0, '/images/tours/ct1.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1, 
N'Ngày 1: Thăm nhà cổ Bình Thủy | Ngày 2: 05h00 Chợ nổi Cái Răng - Vườn Mỹ Khánh - Cafe Cô Ba miền Tây'),
(N'Cần Thơ: Khám phá Rừng Tràm Trà Sư', N'Đi vỏ lãi xuyên rừng ngập mặn.', N'Mạo hiểm hoang dã.', 1500000, N'Cần Thơ', N'An Giang', 1, '2026-06-05', 20, 0, '/images/tours/ct2.jpg', N'Vỏ lãi', 1, 0, GETDATE(), 4, 
N'Sáng: Xuyên rừng tràm | Trưa: Cá lóc nướng trui | Chiều: Ngắm chim về tổ - Cafe Lúa view ruộng'),
(N'Cần Thơ: Food Tour Lẩu Mắm', N'Bánh tằm - Lẩu mắm - Chè.', N'Vị ngon Tây Đô.', 800000, N'Cần Thơ', N'Cần Thơ', 1, '2026-06-08', 25, 0, '/images/tours/ct3.jpg', N'Ghe chèo', 1, 0, GETDATE(), 5, 
N'Trưa: Lẩu mắm Dạ Lý | Tối: Dạo bến Ninh Kiều ăn bánh cống - Cafe du thuyền sông Hậu'),
(N'Cần Thơ: Nghỉ dưỡng Azerai', N'Khu nghỉ dưỡng biệt lập Cồn Ấu.', N'Sang trọng - Riêng tư.', 7500000, N'TP.HCM', N'Cần Thơ', 3, '2026-06-10', 10, 0, '/images/tours/ct4.jpg', N'Cano riêng', 1, 1, GETDATE(), 1, 
N'Ngày 1: Cano đón khách | Ngày 2: Đạp xe quanh cồn - Spa | Ngày 3: Tiễn sân bay'),
(N'Cần Thơ: Team Building Bắt Cá', N'Tát mương bắt cá nông dân.', N'Vui nhộn & Dân dã.', 1100000, N'Cần Thơ', N'Cần Thơ', 1, '2026-06-15', 100, 0, '/images/tours/ct5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'Sáng: Teambuilding thay đồ bà ba | Trưa: Thưởng thức chiến lợi phẩm bắt được | Chiều: Trò chơi dân gian'),

-- =========================================================
-- 14. AN GIANG
-- =========================================================
(N'An Giang: Hành hương Miếu Bà', N'Núi Sam - Rừng Tràm Trà Sư.', N'Tâm linh & Thiên nhiên.', 1600000, N'TP.HCM', N'An Giang', 2, '2026-04-10', 45, 0, '/images/tours/ag1.jpg', N'Xe giường nằm', 1, 1, GETDATE(), 1, 
N'Ngày 1: Viếng Miếu Bà Chúa Xứ đêm | Ngày 2: Rừng tràm Trà Sư - Cafe Cánh Đồng Sen'),
(N'An Giang: Chinh phục Núi Cấm', N'Cáp treo lên đỉnh núi huyền bí.', N'Mạo hiểm - Khám phá.', 1200000, N'An Giang', N'An Giang', 1, '2026-04-12', 30, 0, '/images/tours/ag2.jpg', N'Cáp treo', 1, 0, GETDATE(), 4, 
N'Sáng: Lên đỉnh núi Cấm | Trưa: Bánh xèo rau rừng | Chiều: Check-in Hồ Thủy Liêm'),
(N'An Giang: Food Tour Bún Cá Châu Đốc', N'Bánh bò thốt nốt - Bún cá.', N'Hương vị vùng biên.', 750000, N'An Giang', N'An Giang', 1, '2026-04-15', 20, 0, '/images/tours/ag3.jpg', N'Xe lôi', 1, 0, GETDATE(), 5, 
N'Sáng: Bún cá Châu Đốc | Chiều: Bánh bò thốt nốt nướng | Tối: Cafe tại Victoria Nui Sam view đồng lúa'),
(N'An Giang: Văn hóa Làng Chăm Đa Phước', N'Tìm hiểu dệt thổ cẩm Chăm.', N'Di sản sắc màu.', 1100000, N'An Giang', N'An Giang', 1, '2026-04-18', 25, 0, '/images/tours/ag4.jpg', N'Thuyền gỗ', 1, 1, GETDATE(), 1, 
N'Sáng: Đi thuyền thăm làng Chăm | Trưa: Ăn món Tung Lò Mò | Chiều: Thăm Thánh đường Hồi giáo'),
(N'An Giang: Team Building Núi Sam', N'Chạy bộ lên đỉnh núi săn bình minh.', N'Thử thách & Ý chí.', 1300000, N'An Giang', N'An Giang', 1, '2026-04-20', 50, 0, '/images/tours/ag5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'04h30: Khởi động chân núi | 06h00: Chào cờ trên đỉnh | 08h00: Teambuilding tại khu du lịch'),

-- =========================================================
-- 15. HÀ TĨNH (THÊM CHO ĐỦ 15 TỈNH)
-- =========================================================
(N'Hà Tĩnh: Thiên Cầm Biển Hát', N'Tắm biển & Nghe tích truyện.', N'Nghỉ dưỡng dân dã.', 2200000, N'Hà Nội', N'Hà Tĩnh', 3, '2026-06-20', 30, 0, '/images/tours/ht1.jpg', N'Xe giường nằm', 1, 1, GETDATE(), 1, 
N'Ngày 1: Check-in Thiên Cầm | Ngày 2: Thăm Ngã Ba Đồng Lộc - Cafe Núi Hồng | Ngày 3: Tiễn khách'),
(N'Hà Tĩnh: Trekking Vũ Quang mạo hiểm', N'Khám phá vườn quốc gia Vũ Quang.', N'Thiên nhiên bí ẩn.', 1800000, N'Hà Tĩnh', N'Hà Tĩnh', 2, '2026-06-25', 10, 0, '/images/tours/ht2.jpg', N'Xe Jeep', 1, 0, GETDATE(), 4, 
N'Ngày 1: Đi sâu vào rừng | Ngày 2: Ngắm thác mây - Check-out'),
(N'Hà Tĩnh: Văn hóa Quê hương cụ Nguyễn Du', N'Thăm khu di tích đại thi hào.', N'Văn học - Di sản.', 1200000, N'Hà Tĩnh', N'Hà Tĩnh', 1, '2026-06-28', 40, 0, '/images/tours/ht3.jpg', N'Xe du lịch', 1, 0, GETDATE(), 1, 
N'Sáng: Di tích Nguyễn Du | Trưa: Ăn bánh mướt giò | Chiều: Chùa Hương Tích'),
(N'Hà Tĩnh: Food Tour Kẹo Cu Đơ', N'Thưởng thức bánh đa, cu đơ, mực nhảy.', N'Hương vị quê hương.', 600000, N'Hà Tĩnh', N'Hà Tĩnh', 1, '2026-07-01', 20, 0, '/images/tours/ht4.jpg', N'Xe máy', 1, 0, GETDATE(), 5, 
N'Sáng: Ăn mực nhảy Vũng Áng | Chiều: Học làm kẹo Cu Đơ - Cafe ven hồ Kẻ Gỗ'),
(N'Hà Tĩnh: Team Building Hồ Kẻ Gỗ', N'Trò chơi tập thể tại đập thủy lợi.', N'Sông nước & Kỷ niệm.', 1500000, N'Vinh', N'Hà Tĩnh', 1, '2026-07-05', 80, 0, '/images/tours/ht5.jpg', N'Xe du lịch', 1, 0, GETDATE(), 3, 
N'08h00: Khai mạc Teambuilding | 11h00: Tiệc trưa bên hồ | 14h00: Du ngoạn lòng hồ');

GO
PRINT N'Đã nạp FULL 75 Tour chi tiết cho 15 tỉnh thành du lịch Việt Nam!';

-- NẠP 5 QUỐC GIA NƯỚC NGOÀI X 5 TOUR = 25 TOURS QUỐC TẾ
INSERT INTO [Tours] 
(
    [Name], [ShortDescription], [DetailDescription], [Price], 
    [DepartureLocation], [Destination], [Duration], [DepartureDate], 
    [MaxParticipants], [CurrentParticipants], [ImageUrl], [Transportation], 
    [IsActive], [IsFeatured], [CreatedAt], [CategoryId], [Schedule]
)
VALUES 
-- =========================================================
-- 1. THÁI LAN (Xứ sở Chùa Vàng)
-- =========================================================
(N'Thái Lan: Nghỉ dưỡng Pattaya biển xanh', N'Đảo San Hô - Show chuyển giới Alcazar.', N'Tour du lịch giải trí.', 6990000, N'TP.HCM', N'Thái Lan', 5, '2026-06-15', 30, 0, '/images/tours/tl1.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Bay đến Bangkok - Di chuyển về Pattaya | Trưa: Thưởng thức Lẩu Thái Tomyum | Chiều: Tắm biển Đảo San Hô (Coral Island) | Tối: Xem show Alcazar hoành tráng - Cafe sân thượng Terminal 21'),
(N'Thái Lan: Mạo hiểm Safari World', N'Vườn thú mở lớn nhất Đông Nam Á.', N'Khám phá thiên nhiên hoang dã.', 7500000, N'TP.HCM', N'Thái Lan', 4, '2026-06-20', 25, 0, '/images/tours/tl2.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Sáng: Thăm công viên Safari ngắm hổ, báo | Trưa: Buffet tại công viên | Chiều: Xem show cá heo & Điệp viên 007 | Tối: Cafe After You nổi tiếng - Dạo chợ đêm Jodd Fairs'),
(N'Thái Lan: Văn hóa Chùa Phật Ngọc', N'Cung điện hoàng gia - Wat Phra Kaew.', N'Tìm hiểu tâm linh Thái Lan.', 8200000, N'Hà Nội', N'Thái Lan', 5, '2026-07-01', 20, 0, '/images/tours/tl3.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Thăm Hoàng Cung - Chùa Phật Ngọc | Trưa: Ăn Pad Thai tại phố cổ | Chiều: Dạo thuyền trên sông Chaophraya | Tối: Trải nghiệm Massage Thái cổ truyền - Cafe Starbucks Iconsiam'),
(N'Thái Lan: Food Tour Bangkok', N'Ăn sập ẩm thực đường phố Yaowarat.', N'Thiên đường ăn vặt.', 6500000, N'TP.HCM', N'Thái Lan', 4, '2026-07-10', 15, 0, '/images/tours/tl4.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Chiều: Shopping tại Central World | Tối: Càn quét phố Tàu Yaowarat - Ăn hải sản T&K - Cafe tại nhà cổ 100 năm'),
(N'Thái Lan: Team Building Bangkok-Pattaya', N'Gắn kết tập thể tại bãi biển Pattaya.', N'Năng động & Vui nhộn.', 7900000, N'TP.HCM', N'Thái Lan', 5, '2026-08-01', 100, 0, '/images/tours/tl5.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Ngày 1: Teambuilding bãi biển - Gala Dinner | Ngày 2: Shopping sỉ tại chợ Pratunam - Về'),

-- =========================================================
-- 2. NHẬT BẢN (Xứ sở Hoa Anh Đào)
-- =========================================================
(N'Nhật Bản: Nghỉ dưỡng Núi Phú Sĩ', N'Tắm Onsen - Ngắm núi Phú Sĩ.', N'Kỳ nghỉ đẳng cấp.', 28900000, N'TP.HCM', N'Nhật Bản', 6, '2026-04-10', 20, 0, '/images/tours/jp1.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Thăm làng cổ Oshino Hakkai | Trưa: Thưởng thức mì Ramen vùng núi | Chiều: Tắm khoáng nóng Onsen ngắm Núi Phú Sĩ | Tối: Tiệc tối phong cách Kaiseki - Cafe Blue Bottle Tokyo'),
(N'Nhật Bản: Mạo hiểm Universal Studio', N'Công viên giải trí hàng đầu Osaka.', N'Thế giới phép thuật Harry Potter.', 25500000, N'Hà Nội', N'Nhật Bản', 5, '2026-05-05', 25, 0, '/images/tours/jp2.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Sáng: Khám phá khu vực Super Nintendo World | Trưa: Ăn trưa tại lâu đài Hogwarts | Chiều: Trải nghiệm tàu lượn mạo hiểm | Tối: Cafe tại Dotonbori - Chụp ảnh cùng biển Glico'),
(N'Nhật Bản: Văn hóa Cố đô Kyoto', N'Chùa Vàng - Rừng tre Arashiyama.', N'Tìm về giá trị truyền thống.', 26800000, N'TP.HCM', N'Nhật Bản', 6, '2026-04-15', 20, 0, '/images/tours/jp3.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Thăm Chùa Vàng Kinkakuji | Trưa: Thưởng thức Sushi băng chuyền | Chiều: Đi dạo rừng tre Arashiyama | Tối: Trải nghiệm trà đạo trà xanh Matcha - Cafe Arabica Kyoto'),
(N'Nhật Bản: Food Tour Tokyo đêm', N'Thị trấn điện tử Akihabara - Shinjuku.', N'Ẩm thực & Công nghệ.', 24000000, N'Hà Nội', N'Nhật Bản', 5, '2026-06-12', 15, 0, '/images/tours/jp4.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Chiều: Mua sắm tại phố điện tử Akihabara | Tối: Ăn đồ nướng Yakitori tại ngõ Omoide Yokocho - Thăm tượng mèo 3D Shinjuku'),
(N'Nhật Bản: Team Building DisneySea', N'Giao lưu văn hóa tại Tokyo DisneySea.', N'Sáng tạo & Đẳng cấp.', 29500000, N'TP.HCM', N'Nhật Bản', 6, '2026-08-20', 50, 0, '/images/tours/jp5.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Ngày 1: Teambuilding trong công viên Disney | Ngày 2: Chinh phục tháp Tokyo SkyTree - Gala Dinner món Nhật'),

-- =========================================================
-- 3. HÀN QUỐC (Xứ sở Kim Chi)
-- =========================================================
(N'Hàn Quốc: Nghỉ dưỡng Đảo Nami', N'Phim trường lãng mạn - Nami Island.', N'Mùa thu vàng rực rỡ.', 15900000, N'TP.HCM', N'Hàn Quốc', 5, '2026-10-15', 25, 0, '/images/tours/kr1.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Di chuyển đến đảo Nami bằng phà | Trưa: Thưởng thức gà nướng cay đặc sản | Chiều: Check-in hàng cây ngân hạnh lãng mạn | Tối: Dạo phố Myeongdong - Cafe phong cách K-Pop'),
(N'Hàn Quốc: Mạo hiểm Everland Park', N'T-Express - Tàu lượn gỗ kịch tính.', N'Công viên giải trí số 1.', 14500000, N'Hà Nội', N'Hàn Quốc', 5, '2026-08-05', 30, 0, '/images/tours/kr2.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Sáng: Khám phá vườn thú hoang dã Lost Valley | Trưa: Buffet quốc tế tại Everland | Chiều: Thử thách tàu lượn T-Express | Tối: Ăn thịt nướng BBQ chuẩn Hàn - Cafe Gấu nâu Line Friends'),
(N'Hàn Quốc: Văn hóa Cung điện Gyeongbok', N'Thử trang phục Hanbok - Cố cung.', N'Tìm hiểu lịch sử Joseon.', 16200000, N'TP.HCM', N'Hàn Quốc', 5, '2026-09-01', 20, 0, '/images/tours/kr3.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Mặc Hanbok thăm Cung điện | Trưa: Sâm gà hầm thơm ngon | Chiều: Thăm làng cổ Bukchon Hanok | Tối: Tháp Namsan ngắm ổ khóa tình yêu - Cafe N-Terrace'),
(N'Hàn Quốc: Food Tour Chợ Gwangjang', N'Bánh xèo, lòng lợn, ẩm thực vỉa hè.', N'Thiên đường món ngon.', 13800000, N'Hà Nội', N'Hàn Quốc', 4, '2026-11-20', 15, 0, '/images/tours/kr4.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Chiều: Mua sắm sâm tại chợ sỉ Namdaemun | Tối: Càn quét chợ Gwangjang - Ăn bánh gạo cay - Uống Soju - Cafe tại Bukchon'),
(N'Hàn Quốc: Team Building Seoul Night', N'Gắn kết tập thể giữa lòng Seoul.', N'Hiện đại & Trẻ trung.', 16900000, N'TP.HCM', N'Hàn Quốc', 5, '2026-12-10', 80, 0, '/images/tours/kr5.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Ngày 1: Teambuilding dạo quanh tháp N Seoul | Ngày 2: Gala Dinner kết hợp xem show biểu diễn Nanta'),

-- =========================================================
-- 4. SINGAPORE (Đảo quốc Sư Tử)
-- =========================================================
(N'Singapore: Nghỉ dưỡng Marina Bay Sands', N'Hồ bơi vô cực - Garden by the Bay.', N'Trải nghiệm sang trọng.', 12500000, N'TP.HCM', N'Singapore', 3, '2026-05-20', 25, 0, '/images/tours/sg1.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Thăm Garden by the Bay ngắm siêu cây | Trưa: Thưởng thức cơm gà Hải Nam | Chiều: Tắm hồ bơi vô cực cao nhất thế giới | Tối: Xem show nhạc nước Spectra - Cafe tại sân bay Jewel'),
(N'Singapore: Mạo hiểm Đảo Sentosa', N'Universal Studio - Nhảy dù iFly.', N'Thế giới vui chơi bất tận.', 10800000, N'TP.HCM', N'Singapore', 4, '2026-06-15', 30, 0, '/images/tours/sg2.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Sáng: Khám phá Universal Studio Singapore | Trưa: Ăn trưa tại nhà hàng Malaysia | Chiều: Trải nghiệm nhảy dù iFly - Nhảy Bungee | Tối: Cafe tại bãi biển Siloso Beach'),
(N'Singapore: Văn hóa Phố Tàu & Tiểu Ấn', N'Chùa Răng Phật - Kiến trúc sắc màu.', N'Hòa nhập văn hóa đa sắc tộc.', 9500000, N'Hà Nội', N'Singapore', 4, '2026-07-01', 20, 0, '/images/tours/sg3.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Thăm Chùa Răng Phật Chinatown | Trưa: Ăn tại trung tâm Maxwell Hawker | Chiều: Dạo phố Little India - Phố Ả Rập | Tối: Cafe tại hẻm Haji Lane nghệ thuật'),
(N'Singapore: Food Tour Hawker Centers', N'Cua sốt ớt - Cháo ếch Geylang.', N'Ẩm thực đường phố đỉnh cao.', 8900000, N'TP.HCM', N'Singapore', 3, '2026-08-10', 15, 0, '/images/tours/sg4.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Trưa: Thưởng thức cua sốt ớt tại Newton | Tối: Cháo ếch Geylang - Cafe Starbucks đẹp nhất Singapore tại Fullerton Waterboat House'),
(N'Singapore: Team Building Sentosa Team', N'Đua xe trượt Luge - Gắn kết đội ngũ.', N'Năng động & Phóng khoáng.', 11500000, N'TP.HCM', N'Singapore', 4, '2026-09-05', 150, 0, '/images/tours/sg5.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Ngày 1: Giải mật thư quanh đảo Sentosa | Ngày 2: Gala Dinner du thuyền triệu đô ngắm toàn cảnh đảo quốc'),

-- =========================================================
-- 5. PHÁP (Kinh đô Ánh sáng)
-- =========================================================
(N'Pháp: Nghỉ dưỡng bên dòng sông Seine', N'Tháp Eiffel - Bảo tàng Louvre.', N'Tour lãng mạn mộng mơ.', 65000000, N'TP.HCM', N'Pháp', 7, '2026-09-10', 15, 0, '/images/tours/fr1.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Thăm Tháp Eiffel biểu tượng | Trưa: Ăn món Pháp chính hiệu | Chiều: Du thuyền trên sông Seine | Tối: Cafe tại Cafe de Flore - Thưởng thức rượu vang Pháp'),
(N'Pháp: Mạo hiểm dãy Alps Thụy Sĩ', N'Trượt tuyết tại đỉnh Mont Blanc.', N'Cảm giác mạnh đỉnh cao.', 72000000, N'Hà Nội', N'Pháp', 8, '2026-12-15', 10, 0, '/images/tours/fr2.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Sáng: Lên cáp treo đỉnh Mont Blanc | Trưa: Ăn phô mai nướng Thụy Sĩ | Chiều: Trải nghiệm trượt tuyết chuyên nghiệp | Tối: Nghỉ đêm tại nhà gỗ ấm áp - Cafe nóng bên lò sưởi'),
(N'Pháp: Văn hóa Cung điện Versailles', N'Nghệ thuật kiến trúc hoàng gia.', N'Hành trình di sản UNESCO.', 68000000, N'TP.HCM', N'Pháp', 7, '2026-08-20', 15, 0, '/images/tours/fr3.jpg', N'Máy bay', 1, 1, GETDATE(), 2, 
N'Sáng: Khám phá Cung điện Versailles lộng lẫy | Trưa: Buffet kiểu Pháp | Chiều: Thăm đồi Montmartre - Nhà thờ Đức Bà | Tối: Xem show Moulin Rouge - Cafe Le Consulat'),
(N'Pháp: Food Tour Bánh Macaron & Croissant', N'Thăm các tiệm bánh lâu đời.', N'Hương vị ngọt ngào Paris.', 62000000, N'TP.HCM', N'Pháp', 6, '2026-10-10', 12, 0, '/images/tours/fr4.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Sáng: Ăn sáng Croissant tại tiệm bánh nổi tiếng | Chiều: Thử Macaron tại Ladurée | Tối: Ăn Gan ngỗng béo - Uống rượu vang vùng Bordeaux - Cafe Angelina'),
(N'Pháp: Team Building Sắc màu Paris', N'Cuộc đua kỳ thú quanh Louvre.', N'Trí tuệ & Sang trọng.', 69000000, N'TP.HCM', N'Pháp', 7, '2026-09-01', 30, 0, '/images/tours/fr5.jpg', N'Máy bay', 1, 0, GETDATE(), 2, 
N'Ngày 1: Chạy mật thư quanh bảo tàng Louvre | Ngày 2: Gala Dinner sang trọng tại du thuyền trên sông Seine - Trao giải thưởng');

GO
PRINT N'Đã nạp FULL 25 Tour Quốc tế cho 5 quốc gia hot nhất!';