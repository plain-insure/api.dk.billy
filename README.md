# Description
This project provides an integration with the API of Billy (www.billy.dk).

## Features

* Bills
    * Create
    * Read
    * List
    * Delete
* Invoice
    * Create
    * Read
    * List
    * Delete
* Contact
    * Create
    * Read
    * List
    * Delete

# Installing
Install this using NuGet (latest version)
```
install-package BillyService
```


## Build status
[![Build status](https://ci.appveyor.com/api/projects/status/judlef80gt3t7ltc?svg=true)](https://ci.appveyor.com/project/casperkc/billy)


# Testing
 * Register at www.billy.dk, to get your own test account.
 * Create a new API key in the settings of your account.
 * Add the API key as an envinronment variable with the name BILLY_TEST_APIKEY (remember to restart Visual Studio)
