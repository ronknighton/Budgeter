
    $(document).ready(function () {
        var currentMonth = 0, currentYear = 0;
        var monthYear;
        if (currentMonth == 0)
        {
            $.get('/Transactions/Charts').then(function (response) {
                $('#exp').text('$' + response.totalExpense);
                $('#bud').text('$' + response.totalBudget);
                $('#acc').text('$' + response.totalAcc);
                new Morris.Bar({
                    // ID of the element in which to draw the chart.
                    element: 'chart1',
                    // Chart data records -- each entry in this array corresponds to a point on the chart.
                    data: response.bar,


                    // The name of the data record attribute that contains x-values.
                    xkey: 'Name',
                    // A list of names of data record attributes that contain y-values.
                    ykeys: ['Actual', 'Budgeted'],
                    // Labels for the ykeys -- will be displayed when you hover over the chart.
                    labels: ['Expenses to Date', 'Amount Budgeted'],
                    resize: true,
                    xLabelAngle: 60
                });
                currentMonth = response.currentMonth;
                currentYear = response.currentYear;
                monthYear = response.monthYear;
                
                $('#monthYear').text(response.monthYear);
            });
        }
        //Go back one month
        $('#goback').click(function () {
            $.ajax({
                url: '/Transactions/Charts',
                dataType: "json",
                type: 'GET',
                data:
                    {
                        incM: -1,
                        year: currentYear,
                        month: currentMonth
                    },
                success: (function (response) {
                    $('#chart1').empty();
                    $('#exp').text('$' + response.totalExpense);
                    $('#bud').text('$' + response.totalBudget);
                    $('#acc').text('$' + response.totalAcc);
                    new Morris.Bar({
                        element: 'chart1',
                        data: response.bar,
                        xkey: 'Name',
                        ykeys: ['Actual', 'Budgeted'],
                        labels: ['Expenses to Date', 'Amount Budgeted'],
                        resize: true,
                        xLabelAngle: 60
                    });
                    currentMonth = response.currentMonth;
                    currentYear = response.currentYear;
                    monthYear = response.monthYear;
                    $('#monthYear').text(response.monthYear);
                })
            });

        });
        //Go forward one month
        $('#goforward').click(function () {
            $.ajax({
                url: '/Transactions/Charts',
                dataType: "json",
                type: 'GET',
                data:
                    {
                        incM: 1,
                        year: currentYear,
                        month: currentMonth
                    },
                success: (function (response) {
                    $('#chart1').empty();
                    $('#exp').text('$' + response.totalExpense);
                    $('#bud').text('$' + response.totalBudget);
                    $('#acc').text('$' + response.totalAcc);
                    new Morris.Bar({
                        element: 'chart1',
                        data: response.bar,
                        xkey: 'Name',
                        ykeys: ['Actual', 'Budgeted'],
                        labels: ['Expenses to Date', 'Amount Budgeted'],
                        resize: true,
                        xLabelAngle: 60
                    });
                    currentMonth = response.currentMonth;
                    currentYear = response.currentYear;
                    monthYear = response.monthYear;
                    $('#monthYear').text(response.monthYear);
                })
            });

        });
        //Go back one year
        $('#gobackY').click(function () {
            $.ajax({
                url: '/Transactions/Charts',
                dataType: "json",
                type: 'GET',
                data:
                    {
                        incY: -1,
                        year: currentYear,
                        month: currentMonth
                    },
                success: (function (response) {
                    $('#chart1').empty();
                    $('#exp').text('$' + response.totalExpense);
                    $('#bud').text('$' + response.totalBudget);
                    $('#acc').text('$' + response.totalAcc);
                    new Morris.Bar({
                        element: 'chart1',
                        data: response.bar,
                        xkey: 'Name',
                        ykeys: ['Actual', 'Budgeted'],
                        labels: ['Expenses to Date', 'Amount Budgeted'],
                        resize: true,
                        xLabelAngle: 60
                    });
                    currentMonth = response.currentMonth;
                    currentYear = response.currentYear;
                    monthYear = response.monthYear;
                    $('#monthYear').text(response.monthYear);
                })
            });

        });
        //Go forward one year
        $('#goforwardY').click(function () {
            $.ajax({
                url: '/Transactions/Charts',
                dataType: "json",
                type: 'GET',
                data:
                    {
                        incY: 1,
                        year: currentYear,
                        month: currentMonth
                    },
                success: (function (response) {
                    $('#chart1').empty();
                    $('#exp').text('$' + response.totalExpense);
                    $('#bud').text('$' + response.totalBudget);
                    $('#acc').text('$' + response.totalAcc);
                    new Morris.Bar({
                        element: 'chart1',
                        data: response.bar,
                        xkey: 'Name',
                        ykeys: ['Actual', 'Budgeted'],
                        labels: ['Expenses to Date', 'Amount Budgeted'],
                        resize: true,
                        xLabelAngle: 60
                    });
                    currentMonth = response.currentMonth;
                    currentYear = response.currentYear;
                    monthYear = response.monthYear;
                    $('#monthYear').text(response.monthYear);
                })
            });

        });

        //Categories Pie Chart
        $('.btn.btn-xs.bisque').click(function CategoryChart() {
           
            var cId = $(this).attr("id");
            $.ajax({
                url: '/Transactions/GetCategoryData',
                datatype: "json",
                type: 'GET',
                data:
                    {
                        id: cId,
                        year: currentYear,
                        month: currentMonth
                    }
            }).then(function (response) {
                $('#myChart').remove();
                $('iframe.chartjs-hidden-iframe').remove();
                $('#pieContainer').append('<canvas id="myChart" max-height="400" max-widht="400"><canvas>');
                var ctx = document.getElementById("myChart");
                var myChart = new Chart(document.getElementById("myChart"),
                    {
                        "type": "pie",
                        "data":
                            {
                                "labels": response.Labels,
                                "datasets": [{
                                    "label": "My First Dataset",
                                    "data": response.Data,
                                    "backgroundColor": response.Colors
                                }]
                            }
                    });
                $('#catName').text(response.CatName);
                $('#currentMonth').text(monthYear);
                //$('#currentYear').text(' ' + currentYear);

            })
        });
    
      
    });
       
  
