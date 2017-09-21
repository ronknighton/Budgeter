$(document).ready(function () {
var year = 0;
if (year == 0) {
    $.post('/Transactions/GetMonthly').then(function (response) {
        console.log(response);
        new Morris.Bar({
            // ID of the element in which to draw the chart.
            element: 'MonthlyChart',
            // Chart data records -- each entry in this array corresponds to a point on the chart.
            data: response,
            // The name of the data record attribute that contains x-values.
            xkey: 'month',
            // A list of names of data record attributes that contain y-values.
            ykeys: ['income', 'expense', 'budget'],
            // Labels for the ykeys -- will be displayed when you hover over the chart.
            labels: ['Income to Date', 'Expenses to Date', 'Amount Budgeted'],
            resize: true
        });

        year = response[0]['selectedYear'];
        $('#year').text(year);
    });
}
//Go Back a year
$('#gobackYr').click(function () {
    $.ajax({
        url: '/Transactions/GetMonthly',
        datatype: "json",
        type: 'POST',
        data:
    {
        inc: -1,
        yearNum: year
    }
    }).then(function (response) {
        $('#MonthlyChart').empty();
        console.log(response);
        new Morris.Bar({
            element: 'MonthlyChart',
            data: response,
            xkey: 'month',
            ykeys: ['income', 'expense', 'budget'],
            labels: ['Income to Date', 'Expenses to Date', 'Amount Budgeted'],
            resize: true
        });
        year = response[0]['selectedYear'];
        $('#year').text(year);
    });
})
//Go forward a year
$('#goforwardYr').click(function () {
    $.ajax({
        url: '/Transactions/GetMonthly',
        datatype: "json",
        type: 'POST',
        data:
    {
        inc: 1,
        yearNum: year
    }
    }).then(function (response) {
        $('#MonthlyChart').empty();
        console.log(response);
        new Morris.Bar({
            element: 'MonthlyChart',
            data: response,
            xkey: 'month',
            ykeys: ['income', 'expense', 'budget'],
            labels: ['Income to Date', 'Expenses to Date', 'Amount Budgeted'],
            resize: true
        });
        year = response[0]['selectedYear'];
        $('#year').text(year);
    });
})
});