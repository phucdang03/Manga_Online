$(document).ready(function() {
    // LoadData();
});

function LoadData() {
    $.ajax({
        url: 'http://localhost:5098/Manga/HomeManga',
        type: "GET",
        headers: {"ACCESS_TOKEN" : localStorage.getItem('ACCESS_TOKEN')},
        success: function (response) {
            LoadTopMonthManga(response.topMonthManga);
        }
    });
}
function LoadTopMonthManga(listManga) {
    var html='';
    $(listManga).each(function(index, manga) {
        html += `<div class="item">
            <!-- card -->
            <div class="card card--big">
                <div class="card__cover">
                    <a class="tag-image">
                        <samp>
                            <small>Chương `+manga.countChapter+`</small>
                        </samp>
                    </a>
                    <img class="img-top-month" src="/image/manga-image/`+manga.image+`" alt="">
                    <a href="/Public/DetailManga?id=`+manga.id+`" class="card__play">
                        <i class="icon ion-ios-play"></i>
                    </a>
                </div>
                <div class="card__content">
                    <h3 class="card__title">
                        <a href="/Public/DetailManga?id=`+manga.id+`">`+manga.name+`</a>
                    </h3>
                    <span class="card__category">
                    `+loadCategory(manga.categoryMangas)+`
                    </span>
                    <span class="card__rate">
                        <i class="icon ion-ios-star"></i>`+manga.star+`
                    </span>
                    <span class="card__rate" style="margin-left: 5px;">
                        <i class="icon ion-ios-eye"></i>
                        `+formatView(manga.viewCount)+`
                    </span>
                </div>
            </div>
            <!-- end card -->
        </div>`;
    });
    $('#topMonthManga').html(html);
}
function loadCategory(listCate) {
    var html ='';
    $(listCate).each(function(index,categoryManga) {
        html += `<a href="/Public/CategoriesList?Name=`+categoryManga.id+`">`+categoryManga.name+`</a>`;
    });
    return html;
}
function LoadNewUpdateMangas(listManga) {

}
function LoadNewDoneMangas(listManga) {

}
function LoadTopViewMangas(listManga) {

}