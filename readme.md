SyslogProxy
===

SyslogProxy for Seq is a simple proxy that acts as a remote rsyslog target and stuffs events into Seq.

It requires minimal configuration and is easy to use.

Configuration
===
With rsyslog, usage is as simple as adding a TCP target directing any/all facilities you'd like to the machine where SyslogProxy is running.

Currently, a slightly tweaked template is expected, but we might relax this requirement in the future.

Example rsyslog configuration:

  $template Sane,"%pri% %timestamp:::date-rfc3339% %HOSTNAME% %app-name% %msg%\n"
  *.* @@10.0.0.1:6514;Sane

The following can be specified the app's .config file:

|Key|Description|Default|Example|
|-|-|-|-|
|**SeqServer**|URI pointing to your Seq server. This can include a port if needed. | None | `http://localhost:5341`|
|**MessageTemplate**|Template for the logged message, in Serilog format.|None| `{Hostname}:{ApplicationName} {Message}`|
|**ProxyPort**|The port on which the proxy should listen.|6514|6514|
|**TcpConnectionTimeout**|How long, in seconds, before the proxy assumes a remote syslog connection has dissapeared.|600|600|

Installation
===
Build or grab a release zip. Configure as above. As an admin, run:

SyslogProxy will run standalone or as a windows service. To run standalone, simply run the exe.

To install as a service, run:

  syslogproxy.exe /i

To uninstall the service:

  syslogproxy.exe /u

License
===
Copyright (c) 2015, Assurance Systems, Inc.
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
