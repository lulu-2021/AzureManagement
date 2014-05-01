AzureManagement
===============

Library to Manage Azure Management

At work we have to manage a number of Azure subscriptions each with a large number of cloud services.

So far we have used a range of powershell scripts and the Rest Api. With the recent new addition of the Management
client libraries, it was time to write some updated code to handle this.

This codebase consists of a library with a subset of the functionality and some unit tests. The configuration via
the Azure Publish Settings file and some logging is extracted into two small side projects and injected via TinyIOC at
boot time. There is a sample XML file included in the Unit Test library to show how to load some test data for running
the unit tests.

I intend to extend the library to include the tasks we carry out at work and also intend to add a Nancy based web service
so that we can cache all the data in a database for reporting.

