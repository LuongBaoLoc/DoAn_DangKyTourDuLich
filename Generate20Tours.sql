USE [DoAn_DangKyTourDuLich];
GO

-- Xóa dữ liệu cũ nếu muốn (Bỏ comment nếu cần thiết)
-- DELETE FROM [Tours];
-- DBCC CHECKIDENT ('Tours', RESEED, 0);

INSERT INTO [Tours] 
(
    [Name], [ShortDescription], [DetailDescription], [Price], 
    [DepartureLocation], [Destination], [Duration], [DepartureDate], 
    [MaxParticipants], [CurrentParticipants], [ImageUrl], [Transportation], 
    [IsActive], [IsFeatured], [CreatedAt], [CategoryId]
)
VALUES 
(N'Đà Nẵng - Hội An - Bà Nà Hills', N'Khám phá thành phố đáng sống nhất Việt Nam.', N'Tour 4 ngày 3 đêm khám phá các điểm đến hot nhất miền Trung.', 5990000, N'TP. Hồ Chí Minh', N'Đà Nẵng', 4, '2026-05-01', 30, 0, '/images/tours/danang.jpg', N'Máy bay', 1, 1, GETDATE(), 1),

(N'Phú Quốc - Đảo Ngọc Thiên Đường', N'Nghỉ dưỡng tại đảo ngọc Phú Quốc.', N'Tour 3 ngày 2 đêm thư giãn trên những bãi biển tuyệt đẹp và thưởng thức hải sản.', 4500000, N'TP. Hồ Chí Minh', N'Phú Quốc', 3, '2026-05-15', 25, 0, '/images/tours/phuquoc.jpg', N'Máy bay', 1, 1, GETDATE(), 3),

(N'Sapa - Fansipan - Bản Cát Cát', N'Chinh phục nóc nhà Đông Dương.', N'Tour 3 ngày 2 đêm lên vùng cao Tây Bắc ngắm tuyết rơi và văn hóa bản địa.', 3800000, N'Hà Nội', N'Sapa', 3, '2026-06-01', 20, 0, '/images/tours/sapa.jpg', N'Xe du lịch', 1, 1, GETDATE(), 4),

(N'Nha Trang - Vinpearl Land', N'Tắm biển Nha Trang rực rỡ nắng vàng.', N'Tour 4 ngày 3 đêm trải nghiệm vui chơi giải trí không giới hạn tại Vinpearl.', 4200000, N'TP. Hồ Chí Minh', N'Nha Trang', 4, '2026-05-20', 35, 0, '/images/tours/nhatrang.jpg', N'Máy bay', 1, 1, GETDATE(), 3),

(N'Huế - Cố Đô Di Sản', N'Khám phá cố đô Huế mộng mơ.', N'Tour 3 ngày 2 đêm tham quan Đại Nội, các lăng tẩm và dạo thuyền rồng sông Hương.', 3500000, N'TP. Hồ Chí Minh', N'Huế', 3, '2026-06-10', 25, 0, '/images/tours/hue.jpg', N'Máy bay', 1, 0, GETDATE(), 5),

(N'Đà Lạt - Thành Phố Ngàn Hoa', N'Nghỉ dưỡng thành phố sương mù yên bình.', N'Tour 3 ngày 2 đêm săn mây, dạo chợ đêm và tận hưởng không khí lạnh dịu nhẹ.', 3200000, N'TP. Hồ Chí Minh', N'Đà Lạt', 3, '2026-05-25', 30, 0, '/images/tours/dalat.jpg', N'Xe du lịch', 1, 1, GETDATE(), 1),

(N'Hạ Long - Kỳ quan thiên nhiên', N'Thưởng ngoạn vịnh Hạ Long hùng vĩ.', N'Tour 2 ngày 1 đêm ngủ trên du thuyền và khám phá các hang động kỳ bí.', 2500000, N'Hà Nội', N'Hạ Long', 2, '2026-07-10', 40, 0, '/images/tours/halong.jpg', N'Xe du lịch', 1, 1, GETDATE(), 3),

(N'Thái Lan - Bangkok - Pattaya', N'Mua sắm, ăn uống thả ga tại Xứ sở Chùa Vàng.', N'Tour 5 ngày 4 đêm quốc tế với chi phí cực kỳ hấp dẫn, tham quan chùa thiêng và đảo San Hô.', 6990000, N'TP. Hồ Chí Minh', N'Bangkok', 5, '2026-08-05', 25, 0, '/images/tours/thailan.jpg', N'Máy bay', 1, 1, GETDATE(), 2),

(N'Côn Đảo - Tâm linh & Lịch sử', N'Tham quan các di tích lịch sử oai hùng.', N'Tour 3 ngày 2 đêm thăm nghĩa trang Hàng Dương, nhà tù Côn Đảo và biển xanh tĩnh lặng.', 5900000, N'Cần Thơ', N'Côn Đảo', 3, '2026-06-20', 20, 0, '/images/tours/condao.jpg', N'Tàu cao tốc', 1, 0, GETDATE(), 3),

(N'Mộc Châu - Tà Xùa Mây Ngàn', N'Săn mây Tà Xùa, khám phá cao nguyên Mộc Châu.', N'Tour 3 ngày 2 đêm hòa mình vào thiên nhiên vùng vĩ đồi chè Tây Bắc.', 2800000, N'Hà Nội', N'Mộc Châu', 3, '2026-09-15', 15, 0, '/images/tours/mocchau.jpg', N'Xe du lịch', 1, 1, GETDATE(), 4),

(N'Singapore - Quốc Đảo Sư Tử', N'Khám phá quốc đảo sạch nhất thế giới.', N'Tour 4 ngày 3 đêm chiêm ngưỡng Marina Bay Sands, Gardens by the Bay và Sentosa.', 10500000, N'TP. Hồ Chí Minh', N'Singapore', 4, '2026-07-25', 20, 0, '/images/tours/singapore.jpg', N'Máy bay', 1, 1, GETDATE(), 2),

(N'Nhật Bản - Mùa Hoa Anh Đào', N'Kyoto - Osaka - Tokyo.', N'Tour 6 ngày 5 đêm đón mùa hoa đẹp nhất xứ sở mặt trời mọc.', 25900000, N'Hà Nội', N'Tokyo', 6, '2026-03-25', 25, 0, '/images/tours/nhatban.jpg', N'Máy bay', 1, 1, GETDATE(), 2),

(N'Miền Tây - Cần Thơ - Bến Tre', N'Dạo chợ nổi Cái Răng, thưởng thức trái cây miệt vườn.', N'Tour 2 ngày 1 đêm trải nghiệm lênh đênh sông nước miền Tây Nam Bộ.', 1500000, N'TP. Hồ Chí Minh', N'Cần Thơ', 2, '2026-06-05', 30, 0, '/images/tours/mientay.jpg', N'Xe du lịch', 1, 0, GETDATE(), 5),

(N'Hà Giang - Hoa Tam Giác Mạch', N'Đồng Văn, Mèo Vạc, đèo Mã Pì Lèng.', N'Tour 4 ngày 3 đêm chinh phục các con đèo hiểm trở và ngoạn mục nhất Việt Nam.', 3400000, N'Hà Nội', N'Hà Giang', 4, '2026-10-15', 20, 0, '/images/tours/hagiang.jpg', N'Xe giường nằm', 1, 1, GETDATE(), 4),

(N'Châu Âu - Pháp - Thụy Sĩ - Ý', N'Hành trình mộng mơ qua 3 quốc gia huyền thoại.', N'Tour 10 ngày 9 đêm đưa bạn đến Paris hoa lệ, dãy Alps hùng vĩ và Rome cổ kính.', 59900000, N'TP. Hồ Chí Minh', N'Paris', 10, '2026-09-10', 20, 0, '/images/tours/chauau.jpg', N'Máy bay', 1, 1, GETDATE(), 2),

(N'Ninh Bình - Tràng An - Bái Đính', N'Non nước hữu tình, linh thiêng chốn cửa Phật.', N'Tour 1 ngày chiêm bái chùa lớn nhất Đông Nam Á và ngồi thuyền Tràng An.', 850000, N'Hà Nội', N'Ninh Bình', 1, '2026-05-02', 45, 0, '/images/tours/ninhbinh.jpg', N'Xe du lịch', 1, 0, GETDATE(), 5),

(N'Đài Loan - Đài Bắc - Đài Trung', N'Trải nghiệm văn hóa, ẩm thực chợ đêm xứ Đài.', N'Tour 5 ngày 4 đêm ngắm thả đèn trời Thập Phần và du thuyền hồ Nhật Nguyệt.', 11900000, N'TP. Hồ Chí Minh', N'Đài Bắc', 5, '2026-08-20', 25, 0, '/images/tours/dailoan.jpg', N'Máy bay', 1, 0, GETDATE(), 2),

(N'Phong Nha Kẻ Bàng - Quảng Bình', N'Khám phá hang động kỳ vĩ nhất thế giới.', N'Tour 3 ngày 2 đêm vào sâu trong rùng quốc gia Phong Nha, tắm suối Moọc.', 3800000, N'Đà Nẵng', N'Quảng Bình', 3, '2026-07-05', 25, 0, '/images/tours/phongnha.jpg', N'Xe giường nằm', 1, 1, GETDATE(), 4),

(N'Quy Nhơn - Phú Yên', N'Xứ nẫu thanh bình, biển xanh vẫy gọi.', N'Tour 4 ngày 3 đêm check-in Kỳ Co, Eo Gió, Gành Đá Đĩa đẹp đến nghẹt thở.', 4200000, N'TP. Hồ Chí Minh', N'Quy Nhơn', 4, '2026-06-15', 30, 0, '/images/tours/quynhon.jpg', N'Máy bay', 1, 0, GETDATE(), 3),

(N'Buôn Ma Thuột - Đắk Lắk', N'Khám phá hoang sơ Tây Nguyên đại ngàn.', N'Tour 3 ngày 2 đêm thưởng thức cafe chồn, cưỡi voi Bản Đôn và ngắm thác Dray Nur.', 3100000, N'TP. Hồ Chí Minh', N'Buôn Ma Thuột', 3, '2026-11-05', 25, 0, '/images/tours/buonmathuot.jpg', N'Xe giường nằm', 1, 0, GETDATE(), 4);

GO
PRINT N'Đã chèn thành công 20 tour vào Database!';
