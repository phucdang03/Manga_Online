$(document).ready(function () {
    var MangaId = GetURLParameter('id');
    GetManga(MangaId);
});
const myDiv = document.getElementById("filter-rate");
let openDropdown = false;
myDiv.addEventListener("click", function (event) {
    const myDiv = document.getElementById("dropdown-category");
    openDropdown = !openDropdown;
    if (openDropdown) {
        myDiv.className = "show-dropdown";
        myDiv.classList.remove("hide-dropdown");
    } else {
        myDiv.className = "hide-dropdown";
        myDiv.classList.remove("show-dropdown");
    }
});


const fileInput = document.getElementById('fileInput');
const preview = document.getElementById('preview');

fileInput.addEventListener('change', function () {
    const file = this.files[0];
    const reader = new FileReader();

    reader.addEventListener("load", function () {
        preview.src = reader.result;
    }, false);

    if (file) {
        reader.readAsDataURL(file);
    }
});

const submitBtn = document.getElementById("submitBtn");
const status = document.getElementById("status-manga");
const checkboxes = document.getElementsByName('CategoriesId');
const createdAt = document.getElementById('input-createdAt');
let statusValue = document.getElementById('status-manga-value');

let notiStatus = document.getElementById("noti-status");
let notiCategory = document.getElementById("noti-category");
let notiCreatedAt = document.getElementById("CreatedAt");

submitBtn.addEventListener("click", function (e) {
    e.preventDefault();
    let fail = false;

    notiStatus.innerHTML = "";
    notiCategory.innerHTML = "";
    notiCreatedAt.innerHTML = "";
    $('#name-message').text('');
    $('#AuthorName-message').text('');
    $('#myTextarea-message').text('');

    statusValue.value = status.value;
    var values = [];

    // Lặp qua từng checkbox được chọn và lưu giá trị vào mảng
    for (var i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {
            values.push(checkboxes[i].value);
        }
    }
    if (values.length <= 0) {
        notiCategory.innerHTML = "Cần chọn ít nhất 1 thể loại";
        fail = true;
    }
    if (status.value === 'Trạng thái') {
        notiStatus.innerHTML = "chưa chọn trạng thái";
        fail = true;
    }
    if (!/^\d+$/.test(createdAt.value)) {
        notiCreatedAt.innerHTML = "Năm sai";
        fail = true;
    }
    if ($('#Name').val() === '') {
        $('#name-message').text('không để trống');
        fail = true;
    }
    if ($('#AuthorName').val() === '') {
        $('#AuthorName-message').text('không để trống');
        fail = true;
    }
    if ($('#myTextarea').val() === '') {
        $('#myTextarea-message').text('không để trống');
        fail = true;
    }
    if ($('#status-manga-value').val() === '') {
        $('#myTextarea-message').text('không để trống');
        fail = true;
    }
    if ($('#fileInput')[0].files[0] == null && $('#MangaId').val() === '') {
        $('#fileInput-message').text('Chưa chọn ảnh');
        fail = true;
    }
    if (!fail) {
        CreateManga();
    }
});

function CreateManga() {
    const MangaId = $('#MangaId').val();
    const name = $('#Name').val();
    const authorName = $('#AuthorName').val();
    const myTextarea = $('#myTextarea').val();
    const category = $("input[name='CategoriesId']:checked").map(function () {
        return $(this).val();
    }).get();
    const status = GetCategoryIdByName($('#status-manga-value').val());
    const isActive = $('input[name="IsActive"]:checked').val();
    const createdAt = $('#input-createdAt').val();
    if ($('#fileInput')[0].files[0] != null) {
        var formData = new FormData();
        formData.append('imageFile', $('#fileInput')[0].files[0]);
        AjaxManga(name, authorName, myTextarea, createdAt, isActive, category, status, formData);
    } else {
        if (MangaId !== '') {
            AjaxUpdateManga(MangaId, name, authorName, myTextarea, createdAt, isActive, category, status, null);
        }
    }

}

function GetCategoryIdByName(name) {
    switch (name) {
        case 'Hoàn thành':
            return 1;
        case 'Đang cập nhật':
            return 2;
        case 'Dừng cập nhật':
            return 3;
        case 'Trạng thái':
            return 0;
    }
}

function GetCategoryNameById(id) {
    switch (id) {
        case 1:
            return 'Hoàn thành';
        case 2:
            return 'Đang cập nhật';
        case 3:
            return 'Dừng cập nhật';
        case 0:
            return 'Trạng thái';
    }
}

function AjaxManga(name, authorName, myTextarea, createdAt, isActive, category, status, formData) {
    $.ajax({
        url: 'http://localhost:5098/File/CreateImage',
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        headers: {"ACCESS_TOKEN": localStorage.getItem('ACCESS_TOKEN')},
        success: function (response) {
            if ($('#MangaId').val() === '') {
                AjaxCreateManga(name, authorName, myTextarea, createdAt, isActive, category, status, response.data);
            } else {
                AjaxUpdateManga($('#MangaId').val(), name, authorName, myTextarea, createdAt, isActive, category, status, response.data);
            }
        },
        error: function () {
            ShowNotificationError('Tạo thất bại');
        }
    });
}

function AjaxCreateManga(name, authorName, myTextarea, createdAt, isActive, category, status, image) {
    $.ajax({
        url: 'http://localhost:5098/Manga/CreateManga',
        type: "POST",
        headers: {"ACCESS_TOKEN": localStorage.getItem('ACCESS_TOKEN')},
        data: {
            Name: name,
            AuthorName: authorName,
            Description: myTextarea,
            CreatedAt: createdAt,
            IsActive: isActive,
            CategoriesId: category,
            Status: status,
            Image: image
        }, success: function () {
            ShowNotificationSuccess('Tạo thành công manga');
        }, error: function () {
            ShowNotificationError('Tạo thất bại');
        }
    });
}

function AjaxUpdateManga(mangaId, name, authorName, myTextarea, createdAt, isActive, category, status, image) {
    $.ajax({
        url: 'http://localhost:5098/Manga/UpdateManga',
        type: "PUT",
        headers: {"ACCESS_TOKEN": localStorage.getItem('ACCESS_TOKEN')},
        data: {
            MangaId: mangaId,
            Name: name,
            AuthorName: authorName,
            Description: myTextarea,
            CreatedAt: createdAt,
            IsActive: isActive,
            CategoriesId: category,
            Status: status,
            Image: image
        }, success: function () {
            ShowNotificationSuccess('Cập nhật thành công manga');
        }, error: function () {
            ShowNotificationError('Cập nhật thất bại');
        }
    });
}

function GetManga(id) {
    $('#MangaId').val(id);
    $.ajax({
        url: 'http://localhost:5098/Manga/GetManga?id=' + id,
        headers: {"ACCESS_TOKEN" : localStorage.getItem('ACCESS_TOKEN')},
        type: "GET",
        async: true,
        success: function (response) {
            const manga = response.data;
            $('#Name').val(manga.name);
            $('#AuthorName').val(manga.authorName);
            $('#myTextarea').val(manga.description);
            manga.categoryMangas.forEach(function (category) {
                $('#' + category.id).prop('checked', true);
            });
            $('#status-manga-value').val(GetCategoryNameById(manga.status));
            $('#status-manga').val(GetCategoryNameById(manga.status));
            $('input[name="IsActive"]:radio').filter(function () {
                return $(this).val() === manga.isActive + '';
            }).prop('checked', true);
            var date = new Date(manga.createdAt);
            $('#input-createdAt').val(date.getFullYear());
            $("#preview").attr("src", "http://localhost:5030/image/manga-image/" + manga.image);
        }, error: function () {
        }
    });
}
