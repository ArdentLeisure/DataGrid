function DataGridJS(type, columns) {
    var _currentPage = 1;
    var _pageCount = 20;
    var _type = type;
    var _columns = columns;
    var _sort = columns.split(",")[0];
    var _sortdir = "asc";

    $(document).ready(function ($) {
        $(".datagrid-filter").keyup(function (event) {
            if (event.keyCode == 13) {
                DataGridJS.GetData(1, _pageCount);
            }
        });
    });

    function ShowLoader() {
        $("#datagrid-loader").show();
        $("#datagrid-body").html('');
    }
    function HideLoader() {
        $("#datagrid-loader").hide();
    }

    this.Sort = function (sort) {
        if (_sort == sort) {
            if (_sortdir == "asc") {
                _sortdir = "desc";
            }
            else {
                _sortdir = "asc";
            }
        }
        else {
            _sort = sort;
            _sortdir = "asc";
        }

        this.GetData(_currentPage, _pageCount);
    }

    this.NextPage = function () {
        this.GetData(_currentPage + 1, _pageCount);
    }

    this.PreviousPage = function () {
        this.GetData(_currentPage - 1, _pageCount);
    }

    this.GetData = function (page, count) {
        _currentPage = page;
        if (count != 0) {
            _pageCount = count;
        }

        //Start loader
        ShowLoader();

        var jsonData = {
            'type': _type,
            'columns': _columns,
            'page': page,
            'count': count,
            'sort': _sort,
            'sortdir': _sortdir
        };

        //Filtering
        var columns = _columns.split(",");
        jsonData.filter = new Array();

        for (var i = 0; i < columns.length; i++) {
            jsonData.filter[i] = $("#datagrid-filter-" + i).val();
        }

        $.ajax({
            'url': '/Admin/Data/GetData', //TODO - Rename?
            'type': 'POST',
            'dataType': 'json',
            'data': jsonData,
            'traditional': true,
            'success': function (data) {
                //update 
                $.tmpl($("#datagrid-template").html(), data.Data).appendTo("#datagrid-body");

                $("#datagrid-pager-label").html(data.CurrentPage + " / " + data.TotalPages);

                HideLoader();
            },
            'error': OnUpdateFail
        });
    };

    function OnUpdateFail() {
        //TODO - Better error handling
        alert("OH NOES!");
    }
};