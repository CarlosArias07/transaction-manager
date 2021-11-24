# transaction-manager
.NET Core Web API project running SQLite 

Setting up the SQLite database for the project:
>update-database -Project TransactionManager

To record transactions:
>POST: api/v1/transactions

To update records:
>PUT: api/v1/transactions/{id}

To retrieve invoices:
>GET: api/v1/transactions/invoices/{date1}(2021-11-23 e.g.)/{date2}

To set transactions as paid:
>POST: api/v1/transactions/pay/{id}
