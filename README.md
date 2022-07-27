## email账户

POP3服务器: pop.163.com

SMTP服务器: smtp.163.com

### co_test1@163.com

密码：co_test123

授权码：ASSKUOTNRNQETWPU

```json
{
  "email": "co_test1@163.com",
  "password": "ASSKUOTNRNQETWPU"
}
```

```json
{
  "smtp_host": "smtp.163.com",
  "smtp_port": 25,
  "smtp_ssl": false,
  "pop3_host": "pop3.163.com",
  "pop3_ssl": false,
  "pop3_port": 110
}
```

```json
{
  "smtp_host": "smtp.163.com",
  "smtp_port": 465,
  "smtp_ssl": true,
  "pop3_host": "pop3.163.com",
  "pop3_ssl": true,
  "pop3_port": 995
}
```

```json
{
  "to": ["co_test2@163.com", "co_test1@163.com"],
  "subject": "中文测试2",
  "html_body": "<div style=\"line-height:1.7;color:#000000;font-size:14px;font-family:Arial\"><div><p style=\"margin:0;\"><b>123</b></p><p style=\"margin:0;\"><img src=\"cid:7845ec4$2$18238b9c038$Coremail$co_test2$163.com\" orgwidth=\"591\" orgheight=\"227\" data-image=\"1\" style=\"width: 591px; height: 227px;\"></p><b>1213</b></div><div></div><p style=\"margin: 0;\"><br></p><p style=\"margin: 0;\"><img src=\"https://mimg.127.net/p/js6/lib/htmlEditor/portrait/E-Meow/preview/E-Meow2.png\" style=\"width:40px;height:40px;\"></p><div style=\"position:relative;zoom:1\"><div style=\"clear:both\"></div></div></div>"
}
```

```shell
helo smtp.163.com
auth login
Y29fdGVzdDFAMTYzLmNvbQ==
QVNTS1VPVE5STlFFVFdQVQ==
MAIL FROM: <co_test1@163.com>
RCPT TO: <co_test2@163.com>
DATA
.
```

### co_test2@163.com

密码：co_test123

授权码：ETSTLEASRPGXUWIX