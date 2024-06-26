# UP_bank

Este projeto se trata do controle das operações financeiras de um
banco denominado UP. Os módulos são referentes aos controles de:
1. Clientes
2. Endereço
3. Agencia
4. Contas
5. Solicitações de abertura
6. Realizar Operações Saque, Depósito, Consulta e Pagamento

Cada  módulo possui as seguintes operações de CRUD:
1. Cadastrar (CREATE): Todos os campos serão dados como necessários, salvo os que
forem definidos como nullables.
2. Localizar (READ): um registro específico a ser localizado, preferencialmente por seu
dado que o definem como único.
3. Editar (UPDATE): Alterar os dados de um registro, desde que este não seja uma
informação única.
4. Apagar (DELETE): Remove o registro, movendo-o a uma outra área do banco para
que, se necessário, possa ser recuperado.

### Regras de Negócio
**Entidade Clientes**<br>
O banco atende apenas pessoas físicas. Os clientes menores de 18 anos podem ter conta somente
conjunta com algum maior de idade. Os registros de restrição devem ter apenas as funções de
incluir e remover o cadastro.

**Entidade Agencia**<br>
Somente serão aceitos cadastros de pessoas jurídicas. Toda agência cadastrada deve ter pelo
menos um gerente vinculado. Os registros de restrição devem ter apenas as funções de incluir
e remover o cadastro.

**Entidade Conta**<br>
Toda conta deverá ser liberada pelo gerente da agência. Até isso acontecer, a conta existe
porém é inativa, não podendo fazer nenhum tipo de operação. Após aprovação, as contas
podem ser bloqueadas novamente.

### APIs Utilizadas

**API Endereço**<br>
Consulta a API do VIACEP, a partir do CEP, informando apenas o número e complemento.

**API Clientes**<br>
**API Funcionarios**<br>
**API Contas**<br>
**API Agencia**<br>

# Documentação de Rotas

## Endereços

### 1. Listar todos

- **Endpoint**: `GET /api/Addresses`
- **Descrição**: Retorna os endereços cadastrados.
- **Parâmetros**: Nenhum.
- **Exemplo de Resposta**:
  ```json
    [
        {
            "zipCode": "08565-420",
            "number": "777",
            "street": "Rua Água Vermelha",
            "complement": "sla",
            "city": "Poá",
            "state": "SP"
        },
        {
            "zipCode": "14801-510",
            "number": "11",
            "street": "Avenida Cônego Aldomiro Storniolo",
            "complement": "Casa",
            "city": "Araraquara",
            "state": "SP"
        },
        {
            "zipCode": "14806-099",
            "number": "300",
            "street": "Rua Doutor Emílio Ribas",
            "complement": "Apartment 501",
            "city": "Araraquara",
            "state": "SP"
        },
        {
            "zipCode": "14811-450",
            "number": "10000",
            "street": "Avenida Francisco Martins Caldeira Filho",
            "complement": "Apartment 10",
            "city": "Araraquara",
            "state": "SP"
        },
        {
            "zipCode": "28970-970",
            "number": "10000",
            "street": "Rua Major Félix Moreira",
            "complement": "Apartment 10",
            "city": "Araruama",
            "state": "RJ"
        },
        {
            "zipCode": "74340-080",
            "number": "753",
            "street": "Rua Getúlio Vargas",
            "complement": "Casa B",
            "city": "Goiânia",
            "state": "GO"
        }
    ]
  ```
  
### 2. Encontrar um endereço pelo CEP

- **Endpoint**: `GET /api/Addresses/zipcode/:zipcode/number/:number`
- **Descrição**: Retorna o endereço com base no código postal e número residencial, se cadastrado.
- **Parâmetros**:
    - **zipcode**: string
    - **number**: string
- **Exemplo de Resposta**:
    ```json
    {
        "zipCode": "12410-120",
        "number": "123",
        "street": "Rua Desembargador José Ovídio Marcondes Romeiro",
        "complement": "Casa B",
        "city": "Pindamonhangaba",
        "state": "SP"
    }
    ```
      
### 3. Cadastrar um endereço

- **Endpoint**: `POST /api/Addresses`
- **Descrição**: Cadastra um novo endereço com base no código postal e número.
- **Parâmetros**: Nenhum.
- **Corpo da requisição**:
    ```json
    {
        "zipcode": "12410-120",
        "number": "123",
        "complement": "Casa B" // Opcional
    }
    ```
- **Exemplo de Resposta**:
    ```json
    {
        "zipCode": "12410-120",
        "number": "123",
        "street": "Rua Desembargador José Ovídio Marcondes Romeiro",
        "complement": "Casa B",
        "city": "Pindamonhangaba",
        "state": "SP"
    }
    ```

## Funcionários

### 1. Listar todos

- **Endpoint**: `GET: api/Employees`
- **Descrição**: Retorna os funcionários cadastrados.
- **Parâmetros**: Nenhum.
- **Exemplo de Resposta**:
  ```json
    [
        {
            "manager": true,
            "register": 1,
            "cpf": "563.161.520-78",
            "name": "Brandon",
            "dtBirth": "1808-01-01T00:00:00",
            "sex": "M",
            "income": 50000,
            "phone": "169999999",
            "email": "john.doe@example.com",
            "address": {
                "zipCode": "14811-450",
                "number": "10000",
                "street": "Avenida Francisco Martins Caldeira Filho",
                "complement": "Apartment 10",
                "city": "Araraquara",
                "state": "SP"
            }
      },
        {
            "manager": false,
            "register": 324,
            "cpf": "772.030.950-29",
            "name": "John Peace",
            "dtBirth": "2001-05-22T00:00:00",
            "sex": "M",
            "income": 3000,
            "phone": "11997345678",
            "email": "johnpeace.silva@example.com",
            "address": {
                "zipCode": "74340-080",
                "number": "753",
                "street": "Rua Getúlio Vargas",
                "complement": "Casa B",
                "city": "Goiânia",
                "state": "GO"
            }
        }
    ]
  ```

### 2. Encontrar um funcionário

- **Endpoint**: `GET: api/Employees/:cpf`
- **Descrição**: Retorna o funcionário correspondente ao CPF inserido. O uso do "deleted=false" é necessário para buscar um funcionário deletado. 
- **Parâmetro**:
    - **CPF**: string
- **Exemplo de Resposta**:
    ```json
    {
        "zipcode": "15990-540",
        "number": "5",
        "street": "Avenida Trolesi",
        "complement": "Casa A",
        "city": "Matão",
        "state": "SP"
    }
    ```
  ### 3. Encontrar um funcionário deletado

- **Endpoint**: `GET: api/Employees/:cpf?deleted=true`
- **Descrição**: Retorna o funcionário deletado, correspondente ao CPF inserido.
- **Parâmetros**:
    - **CPF**: string
    - **deleted**: bool
- **Exemplo de Resposta**:
    ```json
    {
        "cpf": "092.818.980-55",
        "name": "Grace Hopper",
        "dtBirth": "2000-01-01T00:00:00",
        "sex": "F",
        "income": 500000,
        "phone": "16912345678",
        "email": "gracehopperSecondEmail.silva@example.com",
        "address": {
            "zipCode": "14801-510",
            "number": "11",
            "street": "Avenida Cônego Aldomiro Storniolo",
            "complement": "Casa",
            "city": "Araraquara",
            "state": "SP"
        },
        "addressZipCode": "14801-510",
        "addressNumber": "11",
        "manager": true,
        "register": 11
  }
    ```

### 4. Cadastrar um funcionário

- **Endpoint**: `POST: api/Employees`
- **Descrição**: Cadastra um novo funcionário.
- **Parâmetros**: Nenhum.
- **Corpo da requisição**:
    ```json
    {
        "Cpf": "772.030.950-29",
        "Name": "John Peace",
        "DtBirth": "2001-05-22",
        "Sex": "M",
        "Income": 3000,
        "Phone": "11997345678",
        "Email": "johnpeace.silva@example.com",
        "AddressDTO": {
            "ZipCode": "74340-080",
            "Number": "753",
            "Complement": "Casa B"
        },
        "Manager": false,
        "Register": 324
    }
    ```
- **Exemplo de Resposta**:
    ```json
    {
        "manager": false,
        "register": 324,
        "cpf": "772.030.950-29",
        "name": "John Peace",
        "dtBirth": "2001-05-22T00:00:00",
        "sex": "M",
        "income": 3000,
        "phone": "11997345678",
        "email": "johnpeace.silva@example.com",
        "address": {
            "zipCode": "74340-080",
            "number": "753",
            "street": "Rua Getúlio Vargas",
            "complement": "Casa B",
            "city": "Goiânia",
            "state": "GO"
        }
    }
    ```

### 5. Definir o perfil de uma conta

- **Endpoint**: `POST: api/Employees`
- **Descrição**: Cadastra um novo funcionário.
- **Parâmetros**: Nenhum.
- **Corpo da requisição**:
    ```json
    {
        "Agency" : "777",
        "OwnerCpf" : "811.201.240-77",
        "DependentCPF" : "",
        "Profile" : "Normal"
    }
    ```
- **Exemplo de Resposta**:
    ```json
    {
        "agency": {
            "number": "777",
            "restriction": false
        },
        "number": "0190",
        "customers": [
            {
                "cpf": "811.201.240-77",
                "dtBirth": "2015-09-30T00:00:00",
                "restriction": false
            }
        ],
        "restriction": true,
        "creditCard": {
            "number": 4231701059993569,
            "expirationDate": "2029-06-01T00:00:00",
            "limit": 5000.5,
            "cvv": "941",
            "name": "Teste",
            "flag": "Visa",
            "active": false
        },
        "overdraft": 1000,
        "savingsAccount": "0190-53",
        "profile": 1,
        "date": "2024-06-26T00:00:00-03:00",
        "balance": 0,
        "extract": null
    }
    ```

### 6. Editar um funcionário

- **Endpoint**: `PUT: api/Employees/:cpf`
- **Descrição**: Edita um funcionário.
- **Parâmetro**:
    - **cpf**: string
- **Corpo da requisição**:
    ```json
    {
        "Cpf": "772.030.950-29",
        "Name": "John Peace",
        "DtBirth": "2001-05-22T00:00:00",
        "Sex": "M",
        "Income": 3500,
        "Phone": "16912345678",
        "Email": "johnpeaceSecondEmail.silva@example.com",
        "Manager": false
    }
    ```
- **Exemplo de Resposta**:
    ```json
    {
        "manager": false,
        "register": 324,
        "cpf": "772.030.950-29",
        "name": "John Peace",
        "dtBirth": "2001-05-22T00:00:00",
        "sex": "M",
        "income": 3500,
        "phone": "16912345678",
        "email": "johnpeaceSecondEmail.silva@example.com",
        "address": null
    }
    ```

### 7. Editar a restricao de uma conta

- **Endpoint**: `PUT: api/Employees/UpdateAccountRestriction/:accountNumber/:employeeCpf/:restriction`
- **Descrição**: Edita a restrição de uma conta.
- **Parâmetros**:
    - **accountNumber**: string 
    - **cpf**: string
    - **restriction**: bool
- **Exemplo de Resposta**:
    ```json
    {
        "number": "0483",
        "agency": {
            "number": "0064",
            "restriction": false
        },
        "customers": [
            {
                "cpf": "811.201.240-77",
                "dtBirth": "2015-09-30T03:00:00Z",
                "restriction": false
            }
        ],
        "restriction": false,
        "creditCard": {
            "number": 5244129819233311,
            "expirationDate": "2029-06-01T03:00:00Z",
            "limit": 20000,
            "cvv": "262",
            "name": "Teste",
            "flag": "MasterCard",
            "active": false
        },
        "overdraft": 3000,
        "profile": 2,
        "date": "2024-06-26T03:00:00Z",
        "balance": 0,
        "extract": null
    }
    ```


### 8. Remover um funcionário

- **Endpoint**: `DELETE: api/Employees/:cpf`
- **Descrição**: Transfere o funcionário para a tabela de funcionários deletados.
- **Parâmetros**:
    - **cpf**: string
- **Exemplo de Resposta**:
    ```json

    {
        "cpf": "772.030.950-29",
        "name": "John Peace",
        "dtBirth": "2001-05-22T00:00:00",
        "sex": "M",
        "income": 3500,
        "phone": "16912345678",
        "email": "johnpeaceSecondEmail.silva@example.com",
        "address": null,
        "addressZipCode": "74340-080",
        "addressNumber": "753",
        "manager": false,
        "register": 324
    }
    
    ```

    
