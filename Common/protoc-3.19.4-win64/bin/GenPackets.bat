protoc.exe -I=./ --csharp_out=./ ./Protocol.proto
dir >log.txt 2>&1 | type log.txt

REM START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
REM XCOPY /Y GenPackets.cs "../../DummyClient/Packet"
REM XCOPY /Y GenPackets.cs "../../Server/Packet"
REM XCOPY /Y ClientPacketManager.cs "../../DummyClient/Packet"
REM XCOPY /Y ServerPacketManager.cs "../../Server/Packet"